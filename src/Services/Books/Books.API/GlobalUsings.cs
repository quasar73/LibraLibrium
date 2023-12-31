﻿global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.EntityFrameworkCore;
global using System.Net;
global using Serilog;
global using LibraLibrium.Services.Books.API.Entities;
global using LibraLibrium.Services.Books.API.Infrastructure.EntityConfigurations;
global using LibraLibrium.Services.Books.API.Infrastructure;
global using LibraLibrium.Services.Books.API.Extensions;
global using System.Globalization;
global using System.Reflection.PortableExecutable;
global using System.Text.RegularExpressions;
global using System.Reflection;
global using Polly;
global using Polly.Retry;
global using Npgsql;
global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
