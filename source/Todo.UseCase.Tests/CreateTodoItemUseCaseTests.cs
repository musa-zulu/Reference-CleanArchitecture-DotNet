﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Domain.Presenter;
using Todo.Domain.Messages;
using Todo.TestUtils;

namespace Todo.UseCase.Tests
{
    [TestFixture]
    public class CreateTodoItemUseCaseTests
    {

        [Test]
        public void Ctor_WhenNullTodoRepository_ShouldThrowArgumentNullException()
        {
            //---------------Arrange-------------------
            //---------------Act-------------------
            var result = Assert.Throws<ArgumentNullException>(() => { new CreateTodoItemUseCase(null); });
            //---------------Assert-------------------
            Assert.AreEqual("respository", result.ParamName);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Execute_WhenEmptyOrWhitespaceItemDescription_ShouldReturnErrorMessage(string itemDescription)
        {
            //---------------Arrange-------------------
            var expected = new List<string> {"ItemDescription cannot be empty or null"};
            var presenter = new PropertyPresenter<CreateTodoItemOuput, ErrorOutputMessage>();
            var usecase = new CreateTodoUseCaseTestDataBuilder().Build();
            var message = CreateTodoItemMessage(itemDescription);
            //---------------Act-------------------
            usecase.Execute(message, presenter);
            //---------------Assert-------------------
            Assert.AreEqual(expected, presenter.ErrorContent.Errors);
        }

        [Test]
        public void Execute_WhenInputMessageContainsValidData_ShouldReturnItemId()
        {
            //---------------Arrange-------------------
            var id = Guid.NewGuid();
            var expected = id;
            var presenter = new PropertyPresenter<CreateTodoItemOuput, ErrorOutputMessage>();
            var usecase = new CreateTodoUseCaseTestDataBuilder()
                            .WithModelId(id)
                            .Build();
            var message = CreateTodoItemMessage("stuff to get done!");
            //---------------Act-------------------
            usecase.Execute(message, presenter);
            //---------------Assert-------------------
            Assert.AreEqual(expected, presenter.SuccessContent.Id);
        }


        private CreateTodoItemInput CreateTodoItemMessage(string itemDescription)
        {
            var message = new CreateTodoItemInput
            {
                ItemDescription = itemDescription,
                DueDate = DateTime.Parse("2017-01-01")
            };
            return message;
        }

    }
}
