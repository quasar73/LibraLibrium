﻿namespace LibraLibrium.Services.Identity.API.Controllers;

public class AccountController : Controller
{
    private readonly ILoginService<ApplicationUser> _loginService;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clientStore;
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AccountController(
        ILoginService<ApplicationUser> loginService,
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        ILogger<AccountController> logger,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _loginService = loginService;
        _interaction = interaction;
        _clientStore = clientStore;
        _logger = logger;
        _userManager = userManager;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (context?.IdP != null)
        {
            throw new NotImplementedException("External login is not implemented!");
        }

        var vm = await BuildLoginViewModelAsync(returnUrl, context);

        ViewData["ReturnUrl"] = returnUrl;

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _loginService.FindByUsername(model.Username);

            if (await _loginService.ValidateCredentials(user, model.Password))
            {
                var tokenLifetime = _configuration.GetValue("TokenLifetimeMinutes", 120);

                var props = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(tokenLifetime),
                    AllowRefresh = true,
                    RedirectUri = model.ReturnUrl
                };

                await _loginService.SignInAsync(user, props);

                if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return Redirect("~/");
            }

            ModelState.AddModelError("", "Invalid username or password.");
        }

        var vm = await BuildLoginViewModelAsync(model);

        ViewData["ReturnUrl"] = model.ReturnUrl;

        return View(vm);
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl, AuthorizationRequest context)
    {
        return new LoginViewModel
        {
            ReturnUrl = returnUrl,
            Username = context?.LoginHint,
        };
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginViewModel model)
    {
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
        var vm = await BuildLoginViewModelAsync(model.ReturnUrl, context);
        vm.Username = model.Username;
        return vm;
    }


    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        if (User.Identity.IsAuthenticated == false)
        {
            return await Logout(new LogoutViewModel { LogoutId = logoutId });
        }

        var vm = new LogoutViewModel
        {
            LogoutId = logoutId
        };
        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutViewModel model)
    {
        var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

        if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
        {
            if (model.LogoutId == null)
            {
                model.LogoutId = await _interaction.CreateLogoutContextAsync();
            }

            string url = "/Account/Logout?logoutId=" + model.LogoutId;

            try
            {
                await HttpContext.SignOutAsync(idp, new AuthenticationProperties
                {
                    RedirectUri = url
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LOGOUT ERROR: {ExceptionMessage}", ex.Message);
            }
        }

        await HttpContext.SignOutAsync();

        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

        HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

        return Redirect(logout?.PostLogoutRedirectUri);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                City = model.User.City,
                Country = model.User.Country,
                Name = model.User.Name,
                State = model.User.State,
                PhoneNumber = model.User.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Errors.Count() > 0)
            {
                AddErrors(result);
                return View(model);
            }
        }

        if (returnUrl != null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return Redirect(returnUrl);
            else
                if (ModelState.IsValid)
                return RedirectToAction("Login", "Account", new { returnUrl });
            else
                return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
