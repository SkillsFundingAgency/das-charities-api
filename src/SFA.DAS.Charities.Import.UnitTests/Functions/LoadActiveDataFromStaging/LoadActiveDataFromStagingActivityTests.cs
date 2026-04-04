using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadActiveDataFromStaging;

public class LoadActiveDataFromStagingActivityTests
{
    [Test]
    public async Task LoadActiveDataFromStagingActivityTests_Run_InvokesRepository()
    {
        Mock<ICharitiesImportRepository> repoMock = new();
        LoadActiveDataFromStagingActivity sut = new(repoMock.Object);

        await sut.LoadActiveDataFromStaging(FunctionsMockingHelper.GetMockedFunctionContextWithLogger().Object);

        repoMock.Verify(x => x.LoadDataFromStagingInToLive(), Times.Once);
    }
}
