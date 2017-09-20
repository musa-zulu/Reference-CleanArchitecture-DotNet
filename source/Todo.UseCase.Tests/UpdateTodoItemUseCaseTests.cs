﻿using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Domain.Presenter;
using Todo.Domain.Messages;
using Todo.Domain.Repository;

namespace Todo.UseCase.Tests
{
    [TestFixture]
    public class UpdateTodoItemUseCaseTests
    {

        [Test]
        public void Ctor_WhenNullTodoRepository_ShouldThrowArgumentNullException()
        {
            //---------------Arrange-------------------
            //---------------Act-------------------
            var result = Assert.Throws<ArgumentNullException>(() => { new UpdateTodoItemUseCase(null); });
            //---------------Assert-------------------
            Assert.AreEqual("todoRepository", result.ParamName);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Execute_WhenEmptyOrWhitespaceItemDescription_ShouldReturnErrorMessage(string itemDescription)
        {
            //---------------Arrange-------------------
            var expected = "ItemDescription cannot be null or empty";
            var itemModel = CreateValidUpdateMessage(itemDescription);
            var usecase = CreateUpdateTodoItemUseCase();
            var presenter = new PropertyPresenter<UpdateTodoItemOutput, ErrorOutputMessage>();
            //---------------Act-------------------
            usecase.Execute(itemModel, presenter);
            //---------------Assert-------------------
            Assert.IsTrue(presenter.ErrorContent.HasErrors);
            Assert.AreEqual(expected, presenter.ErrorContent.Errors.First());
        }

        [Test]
        public void Execute_WhenInputMessageContainsValidData_ShouldReturnItemId()
        {
            //---------------Arrange-------------------
            var expected = "item updated";
            var itemModel = CreateValidUpdateMessage("Updated task");
            var usecase = CreateUpdateTodoItemUseCase();
            var presenter = new PropertyPresenter<UpdateTodoItemOutput, ErrorOutputMessage>();
            //---------------Act-------------------
            usecase.Execute(itemModel, presenter);
            //---------------Assert-------------------
            Assert.AreEqual(expected, presenter.SuccessContent.Message);
        }

        private UpdateTodoItemInput CreateValidUpdateMessage(string itemDescription)
        {
            return new UpdateTodoItemInput
            {
                Id = Guid.NewGuid(),
                DueDate = DateTime.Today,
                ItemDescription = itemDescription,
                IsCompleted = true
            };
        }

        private UpdateTodoItemUseCase CreateUpdateTodoItemUseCase()
        {
            var respository = Substitute.For<ITodoRepository>();
            var usecase = new UpdateTodoItemUseCase(respository);
            return usecase;
        }

    }
}
