using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tedwin.Api.Helpers;
using Tedwin.Api.Model;

namespace Tedwin.Api.Services.AuthInfo;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JWT _jwt;

    public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwt = jwt.Value;
    }

    public async Task<AuthModel> RegisterAsync(RegisterModel model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) is not null)
            return new AuthModel { Message = "Email is already registered!" };

        if (await _userManager.FindByNameAsync(model.UserName) is not null)
            return new AuthModel { Message = "UserName is already registered!" };

        var user = new IdentityUser
        {
            UserName = model.UserName,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(error => error.Description));
            return new AuthModel { Message = errors };
        }

        var roleExists = await _roleManager.RoleExistsAsync("User");
        if (!roleExists)
        {
            var role = new IdentityRole("User");
            await _roleManager.CreateAsync(role);
        }

        await _userManager.AddToRoleAsync(user, "User");

        var jwtSecurityToken = await CreateJwtToken(user);

        return new AuthModel
        {
            Email = user.Email,
            ExpiresOn = jwtSecurityToken.ValidTo,
            IsAuthenticated = true,
            Roles = new List<string> { "User" },
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            UserName = user.UserName
        };
    }

    private async Task<JwtSecurityToken> CreateJwtToken(IdentityUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim("roles", role));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id)
        }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(_jwt.DurationInDays),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }

    public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
    {
        var authModel = new AuthModel();

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            authModel.Message = "Email or Password is incorrect!";
            return authModel;
        }

        var jwtSecurityToken = await CreateJwtToken(user);
        var rolesList = await _userManager.GetRolesAsync(user);

        authModel.IsAuthenticated = true;
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authModel.Email = user.Email;
        authModel.UserName = user.UserName;
        authModel.ExpiresOn = jwtSecurityToken.ValidTo;
        authModel.Roles = rolesList.ToList();

        return authModel;
    }

    public async Task<string> AddRoleAsync(AddRoleModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
            return "Invalid user ID or Role";

        if (await _userManager.IsInRoleAsync(user, model.Role))
            return "User already assigned to this role";

        var result = await _userManager.AddToRoleAsync(user, model.Role);

        return result.Succeeded ? string.Empty : "Something went wrong";
    }
}

//public class AuthService : IAuthService
//{
//    private readonly UserManager<IdentityUser> _userManager;
//    private readonly RoleManager<IdentityRole> _roleManager;
//    private readonly JWT _jwt;

//    public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
//    {
//        _userManager = userManager;
//        _roleManager = roleManager;
//        _jwt = jwt.Value;
//    }
//    public async Task<AuthModel> RegisterAsync(RegisterModel model)
//    {
//        if (await _userManager.FindByEmailAsync(model.Email) is not null)
//            return new AuthModel { Message = "Email is already registerd!" };

//        if (await _userManager.FindByNameAsync(model.UserName) is not null)
//            return new AuthModel { Message = "UserName is already registerd!" };

//        var user = new IdentityUser
//        {
//            UserName = model.UserName,
//            Email = model.Email
//        };
//        var result = await _userManager.CreateAsync(user, model.Password);
//        if (!result.Succeeded)
//        {
//            var errors = string.Empty;

//            foreach (var error in result.Errors)
//            {
//                errors = $"{error.Description},";
//            }
//            return new AuthModel { Message = errors };
//        }

//        await _userManager.AddToRoleAsync(user, "User");

//        var jwtSecurityToken = await CreateJwtToken(user);

//        return new AuthModel
//        {
//            Email = user.Email,
//            ExpiresOn = jwtSecurityToken.ValidTo,
//            IsAuthenticated = true,
//            Roles = new List<string> { "User" },
//            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
//            UserName = user.UserName
//        };
//    }
//    private async Task<JwtSecurityToken> CreateJwtToken(IdentityUser user)
//    {
//        var userClaims = await _userManager.GetClaimsAsync(user);
//        var roles = await _userManager.GetRolesAsync(user);
//        var roleClaims = new List<Claim>();

//        foreach (var role in roles)
//            roleClaims.Add(new Claim("roles", role));

//        var claims = new[]
//        {
//                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//                new Claim(JwtRegisteredClaimNames.Email, user.Email),
//            new Claim("uid", user.Id)
//            }
//        .Union(userClaims)
//        .Union(roleClaims);

//        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
//        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

//        var jwtSecurityToken = new JwtSecurityToken(
//        issuer: _jwt.Issuer,
//            audience: _jwt.Audience,
//            claims: claims,
//            expires: DateTime.Now.AddDays(_jwt.DurationInDays),
//            signingCredentials: signingCredentials);

//        return jwtSecurityToken;
//    }
//    public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
//    {
//        var authModel = new AuthModel();

//        var user = await _userManager.FindByEmailAsync(model.Email);

//        if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
//        {
//            authModel.Message = "Email or Password is incorrect!";
//            return authModel;
//        }

//        var jwtSecurityToken = await CreateJwtToken(user);
//        var rolesList = await _userManager.GetRolesAsync(user);

//        authModel.IsAuthenticated = true;
//        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
//        authModel.Email = user.Email;
//        authModel.UserName = user.UserName;
//        authModel.ExpiresOn = jwtSecurityToken.ValidTo;
//        authModel.Roles = rolesList.ToList();

//        return authModel;
//    }
//    public async Task<string> AddRoleAsync(AddRoleModel model)
//    {
//        var user = await _userManager.FindByIdAsync(model.UserId);

//        if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
//            return "Invalid user ID or Role";

//        if (await _userManager.IsInRoleAsync(user, model.Role))
//            return "User already assigned to this role";

//        var result = await _userManager.AddToRoleAsync(user, model.Role);

//        return result.Succeeded ? string.Empty : "Sonething went wrong";
//    }
//}
