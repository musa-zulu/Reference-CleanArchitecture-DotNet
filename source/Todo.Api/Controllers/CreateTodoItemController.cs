﻿using System.Web.Http;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Presenters;
using Todo.Domain.Messages;
using Todo.Domain.UseCase;

namespace Todo.Api.Controllers
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
        public IHttpActionResult Execute([FromBody] CreateTodoItemInputMessage inputMessage)
        {
            var presenter = new SuccessOrErrorRestfulPresenter<CreateTodoItemOuputMessage, ErrorOutputMessage>(this);
            
            _useCase.Execute(inputMessage, presenter);

            return presenter.Render();
        }
    }
}