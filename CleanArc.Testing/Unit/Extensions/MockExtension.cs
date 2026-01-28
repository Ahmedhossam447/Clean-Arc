using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Models.Identity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Testing.Unit.Extensions
{
    public static class MockExtension
    {
        public  static void MockGetAnimalByIdAsync(this IRepository<Animal> repository,Animal animal)
        {
            repository.GetByIdAsync(animal.AnimalId).Returns(Task.FromResult(animal));
        }
        public static void MockGetUserByIdAsync(this IUserService userService,AuthUser user)
        {
            userService.GetUserByIdAsync(Arg.Is(user.Id)).Returns(Task.FromResult(user));
        }
        public static void MockGetUserByIdAsync(this IUserService userService, string id,string name)
        {
            userService.MockGetUserByIdAsync(new AuthUser { Id = id, UserName = name,Email=$"{name}@test.com" });
        }
    }
}
