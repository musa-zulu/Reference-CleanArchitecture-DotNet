﻿using System.Net;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Presenters;
using Todo.Domain.UseCase;
using Todo.Domain.UseCaseMessages;

namespace Todo.Api.Controllers
{
    [RoutePrefix("todo")]
    public class UpdateTodoItemController : ApiController
    {
        private readonly IUpdateTodoItemUseCase _useCase;

        public UpdateTodoItemController(IUpdateTodoItemUseCase useCase)
        {
            _useCase = useCase;
        }

        [Route("update")]
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UpdateTodoItemOutput))]
        public IHttpActionResult Execute([FromBody] UpdateTodoItemInput inputTo)
        {
            var presenter = new SuccessOrErrorRestfulPresenter<UpdateTodoItemOutput, ErrorOutputMessage>(this);
            
            _useCase.Execute(inputTo, presenter);

            return presenter.Render();
        }
    }
}