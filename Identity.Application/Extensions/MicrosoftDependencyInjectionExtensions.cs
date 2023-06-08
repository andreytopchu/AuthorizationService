﻿using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.Models.Command.Policy;
using Identity.Application.Abstractions.Models.Command.Role;
using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using Identity.Application.Abstractions.Models.Query.Client;
using Identity.Application.Abstractions.Models.Query.Policy;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.UseCases.ApiResource;
using Identity.Application.UseCases.Client;
using Identity.Application.UseCases.Policy;
using Identity.Application.UseCases.Role;
using Identity.Application.UseCases.User;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Extensions;

public static class MicrosoftDependencyInjectionExtensions
{
    public static IServiceCollection AddIdentityUseCases(this IServiceCollection services)
    {
        // automapper
        var currentAssembly = typeof(MicrosoftDependencyInjectionExtensions).Assembly;
        services.AddAutoMapper(expression => expression.AddMaps(currentAssembly));

        // cases
        services.AddScoped<IUseCase<IAddRoleCommand, RoleInfo>, AddRoleUseCase>();
        services.AddScoped<IUseCase<IUpdateRoleCommand, RoleInfo>, UpdateRoleUseCase>();
        services.AddScoped<IUseCase<IDeleteRoleCommand>, DeleteRoleUseCase>();
        services.AddScoped<IUseCase<IAddPolicyCommand, PolicyInfo>, AddPolicyUseCase>();
        services.AddScoped<IUseCase<IUpdatePolicyCommand, PolicyInfo>, UpdatePolicyUseCase>();
        services.AddScoped<IUseCase<IDeletePolicyCommand>, DeletePolicyUseCase>();
        services.AddScoped<IUseCase<IAddClientCommand, ClientInfo>, AddClientUseCase>();
        services.AddScoped<IUseCase<IUpdateClientCommand, ClientInfo>, UpdateClientUseCase>();
        services.AddScoped<IUseCase<IDeleteClientCommand>, DeleteClientUseCase>();
        services.AddScoped<IUseCase<IAddApiResourceCommand, ApiResourceInfo>, AddApiResourceUseCase>();
        services.AddScoped<IUseCase<IUpdateApiResourceCommand, ApiResourceInfo>, UpdateApiResourceUseCase>();
        services.AddScoped<IUseCase<IDeleteApiResourceCommand>, DeleteApiResourceUseCase>();
        services.AddScoped<IUseCase<IAddUserCommand, UserInfo>, AddUserUseCase>();
        services.AddScoped<IUseCase<IUpdateUserCommand, UserInfo>, UpdateUserUseCase>();
        services.AddScoped<IUseCase<IDeleteUserCommand>, DeleteUserUseCase>();

        return services;
    }
}