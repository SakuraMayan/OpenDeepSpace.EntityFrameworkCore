﻿using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using OpenDeepSpace.EntityFrameworkCore;
using OpenDeepSpace.NetCore.Hangfire;
using System.Data.SqlClient;

namespace OpenDeepSpaceEntityFrameworkCore.Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UnitOfWorkController : ControllerBase
    {
        IUnitOfWork unitOfWork;
        //自定义上下文仓储
        IRepository<CustomDbContext, Role> roleRepo;
        IRepository<OtherDbContext, Role> otherRoleRepo;

        IBackgroundJobManager backgroundJobManager;

        IServiceScopeFactory serviceScopeFactory;

        IUnitOfWorkDbContextProvider<CustomDbContext> unitOfWorkDbContextProvider;

        public UnitOfWorkController(IUnitOfWork unitOfWork, IRepository<CustomDbContext, Role> roleRepo, IRepository<OtherDbContext, Role> otherRoleRepo, IBackgroundJobManager backgroundJobManager, IServiceScopeFactory serviceScopeFactory, IUnitOfWorkDbContextProvider<CustomDbContext> unitOfWorkDbContextProvider)
        {
            this.unitOfWork = unitOfWork;
            this.roleRepo = roleRepo;
            this.otherRoleRepo = otherRoleRepo;
            this.backgroundJobManager = backgroundJobManager;
            this.serviceScopeFactory = serviceScopeFactory;
            this.unitOfWorkDbContextProvider = unitOfWorkDbContextProvider;
        }

        [HttpGet]
        public void TestUnitOfWork()
        {



            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            //roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            unitOfWork.Commit();

        }

        [HttpGet]
        public async Task TestUnitOfWorkShareSqlConnection()
        {



            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            //otherRoleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            //触发Job执行
            //await backgroundJobManager.EnqueueAsync(new RoleJobArgs() { });
            backgroundJobManager.EnqueueAsync(new RoleJobArgs() { });

            unitOfWork.Commit();

        }

        /// <summary>
        /// 测试异步工作单元
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task TestUnitOfWorkAsyncOp()
        {
            unitOfWork.Initialize(new UnitOfWorkOptions() { IsTransactional = false });//不开启事务

            await roleRepo.InsertAsync(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            //await otherRoleRepo.InsertAsync(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            await otherRoleRepo.InsertAsync(new Role() { Id = Guid.NewGuid(), RoleName = $"一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            await unitOfWork.CommitAsync();

            //开启独立的工作单元
            using (var scope = serviceScopeFactory.CreateScope())
            { 
                unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                roleRepo = scope.ServiceProvider.GetRequiredService<IRepository<CustomDbContext, Role>>();
                otherRoleRepo = scope.ServiceProvider.GetRequiredService<IRepository<OtherDbContext, Role>>();

                await roleRepo.InsertAsync(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
                await otherRoleRepo.InsertAsync(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

                unitOfWork.Commit();
            }


        }

        /// <summary>
        /// 测试与Ado.Net原生操作共享连接的事务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task TestAdoNetShareConnectionUow()
        {

            //从工作单元上下文提供者中 获取一个数据库上下文事务以及连接
           var dbContext = await unitOfWorkDbContextProvider.GetDbContextAsync();

           //获取事务
           var trans = dbContext.Database.CurrentTransaction?.GetDbTransaction();

            //获取连接
            var conn = dbContext.Database.GetDbConnection();
           
            //ADO.Net原生相应的语句执行
            var command = conn.CreateCommand();
            command.Transaction = trans;
            command.CommandText = "delete from role";
            await command.ExecuteNonQueryAsync();
               
            //EFCore仓储语句执行
            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            //await otherRoleRepo.InsertAsync(new Role() { Id = Guid.NewGuid(), RoleName = $"一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            await unitOfWork.CommitAsync();

        }
    }
}
