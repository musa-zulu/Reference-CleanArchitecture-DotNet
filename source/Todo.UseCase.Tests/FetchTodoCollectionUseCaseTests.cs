﻿using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Domain.Presenter;
using Todo.Domain.Model;
using Todo.Domain.Repository;

namespace Todo.UseCase.Tests
{
    [TestFixture]
    public class FetchTodoCollectionUseCaseTests
    {
        [Test]
        public void Execute_WhenInvoked_ShouldReturnCollectionOfAllItems()
        {
            //---------------Arrange-------------------
            var itemModels = CreateTodoItemModels();
            var expected = itemModels;
            var usecase = CreateFetchTodoCollectionUseCase(itemModels);
            var presenter = new PropertyPresenter<List<TodoItemModel>, ErrorOutputMessage>();
            //---------------Act-------------------
            usecase.Execute(presenter);
            //---------------Assert-------------------
            CollectionAssert.AreEquivalent(expected, presenter.SuccessContent);
        }

        private List<TodoItemModel> CreateTodoItemModels()
        {
            var itemModels = new List<TodoItemModel>
            {
                new TodoItemModel {Id = Guid.NewGuid(), ItemDescription = "task 1", DueDate = DateTime.Today},
                new TodoItemModel {Id = Guid.NewGuid(), ItemDescription = "task 2", DueDate = DateTime.Today}
            };
            return itemModels;
        }

        private FetchTodoCollectionUseCase CreateFetchTodoCollectionUseCase(List<TodoItemModel> itemModels)
        {
            var repository = CreateTodoRepository(itemModels);
            var usecase = new FetchTodoCollectionUseCase(repository);
            return usecase;
        }

        private ITodoRepository CreateTodoRepository(List<TodoItemModel> itemModels)
        {
            var repository = Substitute.For<ITodoRepository>();
            repository.FetchAll()
                      .Returns(itemModels);

            return repository;
        }
    }
}
