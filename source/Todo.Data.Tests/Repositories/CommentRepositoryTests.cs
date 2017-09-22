﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NExpect;
using NUnit.Framework;
using TddBuddy.SpeedySqlLocalDb;
using TddBuddy.SpeedySqlLocalDb.Attribute;
using TddBuddy.SpeedySqlLocalDb.Construction;
using Todo.AutoMapper;
using Todo.Data.Context;
using Todo.Data.EfModels;
using Todo.Data.Repositories;
using Todo.Domain.Repository;
using Todo.Entities;
using Todo.Utils;
using static NExpect.Expectations;

namespace Todo.Data.Tests.Repositories
{
    [Category("Integration")]
    [TestFixture]
    [SharedSpeedyLocalDb(typeof(TodoContext))]
    public class CommentRepositoryTests
    {
        [Test]
        public void Create_WhenValidInputModel_ShouldInsertEntity()
        {
            //---------------Arrange-------------------
            using (var wrapper = new SpeedySqlBuilder().BuildWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var assertContext = CreateDbContext(wrapper);
                var comments = CreateCommentRepository(repositoryDbContext);
                var comment = new TodoComment {Comment = "a comment", TodoItemId = Guid.NewGuid()};
                //---------------Act-------------------
                comments.Create(comment);
                comments.Save();
                //---------------Assert-------------------
                var entity = assertContext.Comments.FirstOrDefault();
                Assert.AreNotEqual(Guid.Empty, entity.Id);
            }
        }

        [Test]
        public void Delete_WhenIdPresent_ShouldReturnTrue()
        {
            //---------------Arrange-------------------
            var id = Guid.NewGuid();
            using (var wrapper = new SpeedySqlBuilder().BuildWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var comments = CreateCommentRepository(repositoryDbContext);
                var comment = new TodoComment { Comment = "a comment", Id = id };
                AddComment(repositoryDbContext, id);
                //---------------Act-------------------
                var result = comments.Delete(comment);
                //---------------Assert-------------------
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void Delete_WhenIdNotPresent_ShouldReturnFalse()
        {
            //---------------Arrange-------------------
            using (var wrapper = new SpeedySqlBuilder().BuildWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var comments = CreateCommentRepository(repositoryDbContext);
                var comment = new TodoComment { Comment = "a comment", Id = Guid.NewGuid() };
                //---------------Act-------------------
                var result = comments.Delete(comment);
                //---------------Assert-------------------
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void FindForItem_WhenIdHasComments_ShouldReturnAllCommentsInDateAscOrder()
        {
            //---------------Arrange-------------------
            var todoItemId = Guid.NewGuid();

            using (var wrapper = new SpeedySqlBuilder().BuildWrapper())
            {
                var insertDbContext = CreateDbContext(wrapper);
                AddManyComments(5, insertDbContext, todoItemId);
                var repositoryDbContext = CreateDbContext(wrapper);
                var expected = CreateExpectedTodoComments(repositoryDbContext);

                var comments = CreateCommentRepository(repositoryDbContext);
                //---------------Act-------------------
                var result = comments.FindForItem(todoItemId);
                //---------------Assert-------------------
                AssertCommentCollectionsMatch(expected, result);
            }
        }

        [Test]
        public void FindForItem_WhenIdHasNoComments_ShouldReturnEmptyList()
        {
            //---------------Arrange-------------------
            var id = Guid.NewGuid();
            using (var wrapper = new SpeedySqlBuilder().BuildWrapper())
            {
                var repositoryDbContext = CreateDbContext(wrapper);
                var comments = CreateCommentRepository(repositoryDbContext);
                //---------------Act-------------------
                var result = comments.FindForItem(id);
                //---------------Assert-------------------
                CollectionAssert.IsEmpty(result);
            }
        }

        private void AssertCommentCollectionsMatch(IList<TodoComment> expected, List<TodoComment> result)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Id, result[i].Id);
                Assert.AreEqual(expected[i].TodoItemId, result[i].TodoItemId);
                Assert.AreEqual(expected[i].Created, result[i].Created);
                Assert.AreEqual(expected[i].Comment, result[i].Comment);
            }
        }

        private IList<TodoComment> CreateExpectedTodoComments(TodoContext repositoryDbContext)
        {
            var efModels = repositoryDbContext.Comments.ToList().OrderBy(x => x.Created.TimeOfDay).ToList();
            var expected = ConvertModelsToDomainEntities(efModels);
            return expected;
        }

        private IList<TodoComment> ConvertModelsToDomainEntities(List<CommentEfModel> efModels)
        {
            var mapper = CreateAutoMapper();
            var result = new List<TodoComment>();

            efModels.ForEach(model =>
            {
                result.Add(mapper.Map<TodoComment>(model));
            });

            return result;
        }

        private void AddManyComments(int total, TodoContext insertDbContext, Guid todoItemId)
        {
            var daysAdd = total;
            for(var i = 0; i < total; i++) 
            {
                AddCommentToDbContextInDescendingDateOrder(insertDbContext, todoItemId);
                daysAdd--;
            }
            insertDbContext.SaveChanges();
        }

        private void AddCommentToDbContextInDescendingDateOrder(TodoContext insertDbContext, Guid todoItemId)
        {
            var id = Guid.NewGuid();
            insertDbContext.Comments.Add(
                new CommentEfModel
                {
                    Id = id,
                    TodoItemId = todoItemId,
                    Comment = "comment " + id
                });
        }

        private void AddComment(TodoContext dbContext, Guid id)
        {
            dbContext.Comments.Add(new CommentEfModel {Id = id});
            dbContext.SaveChanges();
        }

        private TodoContext CreateDbContext(ISpeedySqlLocalDbWrapper wrapper)
        {
            return new TodoContext(wrapper.Connection);
        }

        private ICommentRepository CreateCommentRepository(TodoContext repositoryDbContext)
        {
            var todoItems = new CommentRepository(repositoryDbContext);
            return todoItems;
        }

        private IMapper CreateAutoMapper()
        {
            return new AutoMapperBuilder()
                .WithConfiguration(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CommentEfModel, TodoComment>()
                        .ForMember(x => x.Created, opt => opt.ResolveUsing(src => src.Created.ConvertTo24HourFormatWithSeconds()));
                }))
                .Build();
        }
    }
}
