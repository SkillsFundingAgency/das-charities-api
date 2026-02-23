using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadCharityCommissionsDataInToStaging;

public class ClearStagingDataActivityTests
{
    [Test]
    public async Task ClearStagingDataActivityTests_Run_InvokesRepository()
    {
        Mock<ICharitiesImportRepository> repoMock = new();
        ClearStagingDataActivity sut = new(repoMock.Object);

        await sut.Run(FunctionsMockingHelper.GetMockedFunctionContextWithLogger().Object);

        repoMock.Verify(x => x.ClearStagingData(), Times.Once);
    }
}
