using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using ChatHub.DAL.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest;

[TestClass]
public class DepartmentServiceTest
{
    [TestMethod]
    public async Task GetUsers_By_DepartmentId()
    {
        var authServiceMock = new Mock<IAuthService>();
        List<RegisterModel> registerModels = new List<RegisterModel>();
        registerModels.Add(new RegisterModel()
        {
            Fullname = "Krishna Bk",
            Phone = "9817405449",
            Username = "krishna",
            Email = "krishnabk651@gmail.com",
            RoleId = Guid.NewGuid().ToString(),
            DepartmentId = Guid.NewGuid().ToString()
        });

        string departmentId = "a0a1b0ca-910a-4ad7-bda8-f0db75e04d0a";
        var expectedResult = new ServiceResult<List<RegisterModel>>(true, registerModels);

        authServiceMock.Setup(x => x.GetUsersByDeparmentId(departmentId)).ReturnsAsync(expectedResult);

        var authService = authServiceMock.Object;

        var result = await authService.GetUsersByDeparmentId(departmentId);

        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Data);
    }
}
