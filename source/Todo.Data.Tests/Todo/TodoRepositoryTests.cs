﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
using TddBuddy.DateTime.Extensions;
using TddBuddy.SpeedySqlLocalDb;
using TddBuddy.SpeedySqlLocalDb.Attribute;
using TddBuddy.SpeedySqlLocalDb.Construction;
using Todo.Boundary.Todo.Create;
using Todo.Boundary.Todo.Fetch;
using Todo.Boundary.Todo.Fetch.Filtered;
using Todo.Boundary.Todo.Update;
using Todo.Data.Comment;
using Todo.Data.Todo;

namespace Todo.Data.Tests.Todo
{
    [Category("Integration")]
    [TestFixture]
    [SharedSpeedyLocalDb(typeof(TodoContext))]
    public class TodoRepositoryTests
    {
        [Test]
        public void Create_WhenValidInputModel_ShouldInsertEntity()
        {
            //---------------Arrange-------------------
            using (var wrapper = CreateTransactionalWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var assertContext = CreateDbContext(wrapper);
                var todoItems = CreateTodoItemRepository(repositoryDbContext);
                var todoItem = CreateTodoItem("a thing todo!", DateTime.Today);
                //---------------Act-------------------
                todoItems.Create(todoItem);
                todoItems.Persist();
                //---------------Assert-------------------
                AssertItemWasCreatedSuccessfully(assertContext, todoItem.ItemDescription);
            }
        }

        [Test]
        public void Create_WhenValidInputNullDueDate_ShouldInsertEntity()
        {
            //---------------Arrange-------------------
            using (var wrapper = CreateTransactionalWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var assertContext = CreateDbContext(wrapper);
                var todoItems = CreateTodoItemRepository(repositoryDbContext);
                var todoItem = CreateTodoItem("a thing todo!",null);
                //---------------Act-------------------
                todoItems.Create(todoItem);
                todoItems.Persist();
                //---------------Assert-------------------
                AssertItemWasCreatedSuccessfully(assertContext, todoItem.ItemDescription);
            }
        }

        [Test]
        public void FetchAll_WhenNoItems_ShouldReturnEmptyList()
        {
            //---------------Arrange-------------------
            using (var wrapper = CreateTransactionalWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var todoItems = CreateTodoItemRepository(repositoryDbContext);
                //---------------Act-------------------
                var result = todoItems.FetchAll();
                //---------------Assert-------------------
                CollectionAssert.IsEmpty(result);
            }
        }

        [Test]
        public void FetchAll_WhenManyItems_ShouldReturnAllItems()
        {
            //---------------Arrange-------------------
            using (var wrapper = CreateTransactionalWrapper())
            {
                var entityCount = 5;
                var itemEntities = CreateTodoItemEntityFrameworkEntities(entityCount);
                InsertTodoItems(itemEntities, wrapper);
                InsertComments(itemEntities, wrapper);
                var expected = CreateExpectedFromEntities(wrapper);

                var todoItemRepository = CreateTodoItemRepository(wrapper);
                //---------------Act-------------------
                var result = todoItemRepository.FetchAll();
                //---------------Assert-------------------
                AssertTodoItemsMatchExpected(expected, result);
            }
        }

        [Test]
        public void FetchFiltered_WhenIncludeCompletedFalse_ShouldReturnUncompletedItems()
        {
            //---------------Arrange-------------------
            var input = new TodoFilterInput {IncludedCompleted = false};
            using (var wrapper = CreateTransactionalWrapper())
            {
                var entityCount = 10;
                var itemEntities = CreateTodoItemEntityFrameworkEntities(entityCount);
                
                InsertTodoItems(itemEntities, wrapper);
                InsertComments(itemEntities, wrapper);
                var expected = CreateExpectedForFilter(wrapper);

                var todoItemRepository = CreateTodoItemRepository(wrapper);
                //---------------Act-------------------
                var result = todoItemRepository.FetchFiltered(input);
                //---------------Assert-------------------
                AssertTodoItemsMatchExpected(expected, result);
            }
        }

        [Test]
        public void FetchFiltered_WhenIncludeCompletedTrue_ShouldReturnAllItems()
        {
            //---------------Arrange-------------------
            var input = new TodoFilterInput { IncludedCompleted = true };
            using (var wrapper = CreateTransactionalWrapper())
            {
                var entityCount = 10;
                var itemEntities = CreateTodoItemEntityFrameworkEntities(entityCount);

                InsertTodoItems(itemEntities, wrapper);
                InsertComments(itemEntities, wrapper);
                var expected = CreateExpectedFromEntities(wrapper);

                var todoItemRepository = CreateTodoItemRepository(wrapper);
                //---------------Act-------------------
                var result = todoItemRepository.FetchFiltered(input);
                //---------------Assert-------------------
                AssertTodoItemsMatchExpected(expected, result);
            }
        }

        private List<TodoTo> CreateExpectedForFilter(ISpeedySqlLocalDbWrapper wrapper)
        {
            var expected = CreateExpectedFromEntities(wrapper);
            expected = expected.Where(x => x.IsCompleted == false).ToList();
            return expected;
        }

        [Test]
        public void Update_WhenValidInputModel_ShouldUpdateEntity()
        {
            //---------------Arrange-------------------
            using (var wrapper = CreateTransactionalWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var assertContext = CreateDbContext(wrapper);
                var todoItems = CreateTodoItemRepository(repositoryDbContext);
                var id = InsertNewTodoEntity(repositoryDbContext);
                var model = CreateTodoItem(id);
                
                //---------------Act-------------------
                todoItems.Update(model);
                todoItems.Persist();
                //---------------Assert-------------------
                var entity = assertContext.TodoItem.First(x => x.Id == id);

                AssertModelMatchesEntity(model, entity);
            }
        }

        [Test]
        public void Delete_WhenIdExist_ShouldDeleteEntity()
        {
            //---------------Arrange-------------------
            using (var wrapper = CreateTransactionalWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var todoItems = CreateTodoItemRepository(repositoryDbContext);
                var id = InsertNewTodoEntity(repositoryDbContext);
                //---------------Act-------------------
                var result = todoItems.MarkForDelete(id);
                //---------------Assert-------------------
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void Delete_WhenIdDoesNotExist_ShouldNotDeleteEntity()
        {
            //---------------Arrange-------------------
            var id = Guid.NewGuid();

            using (var wrapper = CreateTransactionalWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var todoItems = CreateTodoItemRepository(repositoryDbContext);
                //---------------Act-------------------
                var result = todoItems.MarkForDelete(id);
                //---------------Assert-------------------
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void FindById_WhenIdExist_ShouldReturnDomainEntity()
        {
            //---------------Arrange-------------------
            var id = Guid.NewGuid();

            using (var wrapper = CreateTransactionalWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var todoItems = CreateTodoItemRepository(repositoryDbContext);
                AddEfEntity(repositoryDbContext, id);
                //---------------Act-------------------
                var result = todoItems.FindById(id);
                //---------------Assert-------------------
                Assert.AreEqual(id, result.Id);
            }
        }

        [Test]
        public void FindById_WhenIdNotExist_ShouldReturnMissingDomainEntity()
        {
            //---------------Arrange-------------------
            var id = Guid.NewGuid();

            using (var wrapper = CreateTransactionalWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var todoItems = CreateTodoItemRepository(repositoryDbContext);
                //---------------Act-------------------
                var result = todoItems.FindById(id);
                //---------------Assert-------------------
                Assert.IsNull(result);
            }
        }

        private static ISpeedySqlLocalDbWrapper CreateTransactionalWrapper()
        {
            return new SpeedySqlBuilder().BuildWrapper();
        }

        private void AssertTodoItemsMatchExpected(IReadOnlyList<TodoTo> expected, IReadOnlyList<TodoTo> result)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Id, result[i].Id);
                var expectedComments = expected[i].Comments;
                for (var z = 0; z < expectedComments.Count; z++)
                {
                    Assert.AreEqual(expected[i].Comments[z].Id, result[i].Comments[z].Id);
                    Assert.AreEqual(expected[i].Comments[z].Comment, result[i].Comments[z].Comment);
                }
            }
        }

        private TodoRepository CreateTodoItemRepository(ISpeedySqlLocalDbWrapper wrapper)
        {
            var repositoryDbContext = CreateDbContext(wrapper);
            var todoItemRepository = CreateTodoItemRepository(repositoryDbContext);
            return todoItemRepository;
        }

        private void AddEfEntity(TodoContext repositoryDbContext, Guid id)
        {
            repositoryDbContext.TodoItem.Add(CreateEfEntity(id));
            repositoryDbContext.SaveChanges();
        }

        private TodoItemEntityFrameworkModel CreateEfEntity(Guid id)
        {
            return new TodoItemEntityFrameworkModel
            {
                Id = id,
                DueDate = DateTime.Now,
                Created = DateTime.Now,
                Modified = DateTime.Now,
                ItemDescription = "do stuff"
            };
        }

        private UpdateTodoInput CreateTodoItem(Guid id)
        {
            var model = new UpdateTodoInput
            {
                Id = id,
                ItemDescription = "updated",
                IsCompleted = true,
                DueDate = DateTime.Today
            };
            return model;
        }

        private void AssertModelMatchesEntity(UpdateTodoInput model, TodoItemEntityFrameworkModel entity)
        {
            Assert.AreEqual(model.Id, entity.Id);
            Assert.AreEqual(model.ItemDescription, entity.ItemDescription);
            Assert.AreEqual(model.IsCompleted, entity.IsCompleted);
        }

        private TodoRepository CreateTodoItemRepository(TodoContext repositoryDbContext)
        {
            var todoItems = new TodoRepository(repositoryDbContext);
            return todoItems;
        }

        private Guid InsertNewTodoEntity(TodoContext repositoryDbContext)
        {
            var todoDbEntity = new TodoItemEntityFrameworkModel
            {
                ItemDescription = "new item",
                DueDate = DateTime.Today,
                IsCompleted = false
            };
            repositoryDbContext.TodoItem.Add(todoDbEntity);
            repositoryDbContext.SaveChanges();

            return todoDbEntity.Id;
        }

        private List<TodoTo> CreateExpectedFromEntities(ISpeedySqlLocalDbWrapper wrapper)
        {
            var result = new List<TodoTo>();
            var fetchContext = CreateDbContext(wrapper);
            var todoItems = fetchContext.TodoItem.Include(x => x.Comments).ToList();

            todoItems.ForEach(item =>
            {
                var comments = ConvertToCommentToCollection(item);
                result.Add(CreateTodoItemTo(item, comments));
            });

            return result;
        }

        private TodoTo CreateTodoItemTo(TodoItemEntityFrameworkModel item, List<TodoCommentTo> comments)
        {
            return new TodoTo
            {
                Id = item.Id,
                ItemDescription = item.ItemDescription,
                DueDate = item.DueDate?.ConvertTo24HourFormatWithSeconds(),
                IsCompleted = item.IsCompleted,
                Comments = comments
            };
        }

        private List<TodoCommentTo> ConvertToCommentToCollection(TodoItemEntityFrameworkModel item)
        {
            var comments = new List<TodoCommentTo>();

            item.Comments.ToList().ForEach(c =>
            {
                comments.Add(new TodoCommentTo
                {
                    Id = c.Id,
                    Comment = c.Comment,
                    Created = c.Created.ConvertTo24HourFormatWithSeconds(),
                });
            });
            return comments;
        }

        private void InsertComments(List<TodoItemEntityFrameworkModel> itemEntities, ISpeedySqlLocalDbWrapper wrapper)
        {
            var commentCount = new Random().Next(1,5);
            
            var insertContext = CreateDbContext(wrapper);
            itemEntities.ForEach(item =>
            {
                var itemId = item.Id;
                AddCommentsForTodoItem(commentCount, insertContext, itemId);
            });
            insertContext.SaveChanges();
        }

        private void AddCommentsForTodoItem(int commentCount, TodoContext insertContext, Guid itemId)
        {
            for (var i = 0; i < commentCount; i++)
            {
                var commentId = Guid.NewGuid();
                insertContext.Comments.Add(new CommentEntityFrameworkModel
                {
                    Id = Guid.NewGuid(),
                    Comment = "comment_" + i + " " + commentId,
                    TodoItemId = itemId
                });
            }
        }

        private void InsertTodoItems(List<TodoItemEntityFrameworkModel> items, ISpeedySqlLocalDbWrapper wrapper)
        {
            var insertContext = CreateDbContext(wrapper);
            items.ForEach(item =>
            {
                insertContext.TodoItem.Add(item);
            });
            insertContext.SaveChanges();
        }

        private List<TodoItemEntityFrameworkModel> CreateTodoItemEntityFrameworkEntities(int count)
        {
            var result = new List<TodoItemEntityFrameworkModel>();
            
            
            for (var i = 0; i < count; i++)
            {
                var itemId = Guid.NewGuid();

                var item = new TodoItemEntityFrameworkModel
                {
                    Id = itemId,
                    ItemDescription = $"task #{i+1}",
                    DueDate = DateTime.Today,
                    IsCompleted = GetIsCompleted(i)
                };
                result.Add(item);
            }
            
            return result;
        }

        private static bool GetIsCompleted(int i)
        {
            var isCompleted = i % 2 == 0;

            return isCompleted;
        }

        private void AssertItemWasCreatedSuccessfully(TodoContext assertContext, string expectedDescription)
        {
            var entity = assertContext.TodoItem.FirstOrDefault();
            Assert.AreEqual(expectedDescription, entity?.ItemDescription);
            Assert.IsFalse(entity.IsCompleted);
            Assert.AreNotEqual(Guid.Empty, entity.Id);
        }

        private CreateTodoInput CreateTodoItem(string itemDescription, DateTime? dueDate)
        {
            var inputMessage = new CreateTodoInput
            {
                ItemDescription = itemDescription,
                DueDate = dueDate
            };
            return inputMessage;
        }

        private TodoContext CreateDbContext(ISpeedySqlLocalDbWrapper wrapper)
        {
            return new TodoContext(wrapper.Connection);
        }
    }
}
