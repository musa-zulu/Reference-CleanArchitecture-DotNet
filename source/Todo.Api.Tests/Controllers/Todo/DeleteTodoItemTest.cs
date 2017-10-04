﻿using System;
using System.Net;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using TddBuddy.CleanArchitecture.TestUtils.Builders;
using TddBuddy.CleanArchitecture.TestUtils.Factories;
using Todo.Boundry.Todo.Delete;
using Todo.Domain.Tests.Todo.Delete;
using Todo.Web.Controllers.Todo;

namespace Todo.Web.Controllers.Tests.Controllers.Todo
{
    [TestFixture]
    public class DeleteTodoItemTest
    {
        [Test]
        public void Execute_WhenValidItemId_ShouldReturnSuccess()
        {
            //---------------Arrange-------------------
            var deleteId = Guid.NewGuid();
            var requestUri = $"todo/delete/{deleteId}";

            using (var testServer = CreateTestServer(true))
            {
                var client = TestHttpClientFactory.CreateClient(testServer);
                //---------------Act-------------------
                var response = client.DeleteAsync(requestUri).Result;
                //---------------Assert-------------------
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }            
        }

        [Test]
        public void Execute_WhenInvalidItemId_ShouldReturnUnprocessableEntityCode()
        {
            //---------------Arrange-------------------
            var deleteId = Guid.NewGuid();
            var requestUri = $"todo/delete/{deleteId}";

            using (var testServer = CreateTestServer(false))
            {
                var client = TestHttpClientFactory.CreateClient(testServer);
                //---------------Act-------------------
                var response = client.DeleteAsync(requestUri).Result;
                //---------------Assert-------------------
                Assert.AreEqual((HttpStatusCode)422, response.StatusCode);
            }
        }

        private TestServer CreateTestServer(bool deleteResult)
        {
            var usecase = new DeleteTodoItemUseCaseTestDataBuilder().WithDeleteResult(deleteResult).Build();
            var testServer = new TestServerBuilder<DeleteTodoItem>()
                .WithInstanceRegistration<IDeleteTodoItemUseCase>(usecase)
                .Build();
            return testServer;
        }
    }
}