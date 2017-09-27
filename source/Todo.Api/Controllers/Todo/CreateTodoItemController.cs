﻿using System.Net;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Presenters;
using Todo.Boundry.UseCase;
using Todo.Domain.UseCaseMessages;

namespace Todo.Api.Controllers.Todo
{
    [RoutePrefix("todo")]
    public class CreateTodoItemController : ApiController
    {
        private readonly ICreateTodoItemUseCase _useCase;

        public CreateTodoItemController(ICreateTodoItemUseCase useCase)
        {
            _useCase = useCase;
        }

        [Route("create")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CreateTodoItemOuput))]
        public IHttpActionResult Execute([FromBody] CreateTodoItemInput input)
        {
            var presenter = CreatePresenter();
            
            _useCase.Execute(input, presenter);

            return presenter.Render();
        }

        private SuccessOrErrorRestfulPresenter<CreateTodoItemOuput, ErrorOutputMessage> CreatePresenter()
        {
            var presenter = new SuccessOrErrorRestfulPresenter<CreateTodoItemOuput, ErrorOutputMessage>(this);
            return presenter;
        }
    }
}
