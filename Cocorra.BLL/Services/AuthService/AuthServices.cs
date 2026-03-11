using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cocorra.BLL.DTOS.Auth;
using Cocorra.BLL.Services.Auth;
using Cocorra.BLL.Services.Upload;
using Cocorra.DAL.Data;
using Cocorra.DAL.Enums;
using Cocorra.DAL.Models;
using Core.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cocorra.BLL.Services.AuthServices
{
    public class AuthServices : ResponseHandler, IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager; // إضافة RoleManager
        private readonly IConfiguration _configuration;
        private readonly IUploadVoice _uploadVoice;
        private readonly AppDbContext _context;
        public AuthServices(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager, // حقن RoleManager
            IConfiguration configuration,
            IUploadVoice uploadVoice,
            AppDbContext context)
        {
            _context = context;
            _uploadVoice = uploadVoice;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<Response<AuthModel>> RegisterAsync(RegisterDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
    {
        // نبدأ الترانزكشن هنا
        using var transaction = await _context.Database.BeginTransactionAsync();
        string? voicePathToDelete = null;

        try
        {
            // أ. التحقق من وجود الإيميل (مش محتاج ترانزكشن لأنه قراءة بس)
            var existingUser = await _userManager.FindByEmailAsync(dto.Email!);
            if (existingUser is not null)
            {
                return BadRequest<AuthModel>("Email is already registered");
            }

            // ب. رفع ملف الصوت
            var voicePath = await _uploadVoice.SaveVoice(dto.VoiceVerification!);
            if (voicePath.StartsWith("Error"))
            {
                return BadRequest<AuthModel>(voicePath);
            }
            voicePathToDelete = voicePath; // نمسك المسار عشان لو فشلنا نمسحه

            // ج. تجهيز المستخدم
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Age = dto.Age,
                MBTI = dto.MBTI,
                Status = UserStatus.Pending,
                VoiceVerificationPath = voicePath
            };

            // د. حفظ المستخدم (هنا Identity بيحفظ في الداتابيز بس الترانزكشن ماسكه)
            var result = await _userManager.CreateAsync(user, dto.Password!);
            if (!result.Succeeded)
            {
                // لو فشل الإنشاء، لازم نمسح ملف الصوت اللي اترفع
                DeleteFile(voicePathToDelete);
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest<AuthModel>("Registration failed", errors);
            }

            // هـ. إضافة الرول
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                throw new Exception("Failed to assign role"); // عشان ينزل للـ Catch ويعمل Rollback
            }

            // و. توليد التوكن
            var jwtToken = await GenerateJwtToken(user);
            var authModel = new AuthModel
            {
                Email = user.Email,
                Username = user.UserName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" }
            };

            // ز. اعتماد الترانزكشن (Save نهائي)
            await transaction.CommitAsync();

            return Created(authModel);
        }
        catch (Exception ex)
        {
            // ح. في حالة حدوث أي خطأ مفاجئ
            await transaction.RollbackAsync(); // الغي أي حاجة حصلت في الداتابيز
            DeleteFile(voicePathToDelete); // امسح الملف الصوتي
            return BadRequest<AuthModel>("Something went wrong: " + ex.Message);
        }
    });
        }

        // دالة مساعدة لحذف الملف
        private void DeleteFile(string? path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // تحتاج تحول المسار النسبي لمسار كامل (Full Path)
                // ممكن تحتاج IWebHostEnvironment هنا، أو تمرر الـ Path كامل من خدمة الرفع
                // للتسهيل، سنفترض أن الخدمة بترجع Path نسبي ونحاول نحذفه
                try
                {
                    // هذا مجرد مثال، يفضل ضبط المسار حسب بيئتك
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch { /* تجاهل أخطاء الحذف */ }
            }
        }
        private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = Encoding.ASCII.GetBytes(_configuration["JWTSetting:securityKey"]!);
            var authKey = new SymmetricSecurityKey(key);

            var token = new JwtSecurityToken(
                audience: _configuration["JWTSetting:ValidAudience"], // تأكد إن القيم دي في appsettings.json
                issuer: _configuration["JWTSetting:ValidIssuer"],
                expires: DateTime.UtcNow.AddDays(15),      // استخدم UTC دائماً
                claims: claims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        public async Task<Response<AuthModel>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email!);
            if (user == null)
            {
                return BadRequest<AuthModel>("Invalid Email or Password");
            }
            var result = await _userManager.CheckPasswordAsync(user, dto.Password!);
            if (!result)
            {
                return BadRequest<AuthModel>("Invalid Email or Password");
            }
            switch (user.Status)
            {
                case UserStatus.Pending:
                    return BadRequest<AuthModel>("Your account is still pending approval.");
                case UserStatus.Rejected:
                    return BadRequest<AuthModel>("Your account has been rejected.");
                case UserStatus.Active:
                    break;
                default:
                    return BadRequest<AuthModel>("Invalid user status.");
            }

            var jwtToken = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            var authModel = new AuthModel
            {
                Email = user.Email,
                Username = user.UserName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
                IsAuthenticated = true,
                Roles = roles.ToList()
            };
            return Success(authModel);
        }
    }
}