﻿using System.Web.Http;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using Todo.Boundary.Comment;
using Todo.Boundary.Comment.Create;
using Todo.Boundary.Comment.Delete;
using Todo.Boundary.Todo;
using Todo.Boundary.Todo.Create;
using Todo.Boundary.Todo.Delete;
using Todo.Boundary.Todo.Fetch;
using Todo.Boundary.Todo.Update;
using Todo.Data.Context;
using Todo.Data.Repositories;
using Todo.Domain.Comment.Create;
using Todo.Domain.Comment.Delete;
using Todo.Domain.Todo.Create;
using Todo.Domain.Todo.Delete;
using Todo.Domain.Todo.Fetch;
using Todo.Domain.Todo.Update;

namespace Todo.Api
{
    public static class IocConfig
    {
        public static void Configure(HttpConfiguration configuration)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.RegisterWebApiControllers(configuration);

            RegisterContext(container);
            RegisterRepositories(container);
            RegisterUseCases(container);

            container.Verify();

            configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static void RegisterRepositories(Container container)
        {
            container.Register<ITodoRepository, TodoItemRepository>();
            container.Register<ICommentRepository, CommentRepository>();
        }

        private static void RegisterUseCases(Container container)
        {
            container.Register<ICreateTodoItemUseCase, CreateTodoItemUseCase>();
            container.Register<IFetchTodoCollectionUseCase, FetchTodoCollectionUseCase>();
            container.Register<IDeleteTodoItemUseCase, DeleteTodoItemUseCase>();
            container.Register<IUpdateTodoItemUseCase, UpdateTodoItemUseCase>();

            container.Register<ICreateCommentUseCase, CreateCommentUseCase>();
            container.Register<IDeleteCommentUseCase, DeleteCommentUseCase>();
        }

        private static void RegisterContext(Container container)
        {
            container.Register<TodoContext>(() => new TodoContext(), Lifestyle.Scoped);
        }
    }
}