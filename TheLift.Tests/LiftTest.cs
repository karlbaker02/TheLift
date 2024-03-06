using FluentAssertions;

namespace TheLift.Tests;

[TestClass]
public class LiftTest
{
    private const int DefaultCapacity = 5;

    [TestMethod]
    public void Run_VisitsGroundFloorOnly_WhenNoCallRequestsHaveBeenMade()
    {
        int[][] queues =
        {
            [], // G
            [], // 1
            [], // 2
            [], // 3
            [], // 4
            [], // 5
            [], // 6
        };

        int[] floorsVisited = Lift.Run(queues, DefaultCapacity);

        floorsVisited.Should().BeEquivalentTo(new[] { 0 } );
    }

    [TestMethod]
    public void Run_VisitsASingleFloor_WhenASingleCallIsMade()
    {
        int[][] queues =
        {
            [], // G
            [], // 1
            [4], // 2
            [], // 3
            [], // 4
            [], // 5
            [], // 6
        };

        int[] floorsVisited = Lift.Run(queues, DefaultCapacity);

        floorsVisited.Should().BeEquivalentTo(new[] { 0, 2, 4, 0 });
    }

    [TestMethod]
    public void Run_VisitsMultipleFloors_WhenMultipleCallsAreMade()
    {
        int[][] queues =
        {
            [], // G
            [3], // 1
            [4], // 2
            [], // 3
            [5], // 4
            [], // 5
            [], // 6
        };

        int[] floorsVisited = Lift.Run(queues, DefaultCapacity);

        floorsVisited.Should().BeEquivalentTo(new[] { 0, 1, 2, 3, 4, 5, 0 });
    }

    [TestMethod]
    public void Run_CollectsFromTopFloorDown_WhenInitiallyOnGroundAndAllPassengersGoingDown()
    {
        int[][] queues =
        {
            [], // G
            [0], // 1
            [], // 2
            [], // 3
            [2], // 4
            [3], // 5
            [], // 6
        };

        int[] floorsVisited = Lift.Run(queues, DefaultCapacity);

        floorsVisited.Should().BeEquivalentTo(new[] { 0, 5, 4, 3, 2, 1, 0 });
    }

    [TestMethod]
    public void Run_CollectsPeopleFromGroundFloorAndVisitsMultipleFloors()
    {
        int[][] queues =
        {
            [2, 1, 4], // G
            [0], // 1
            [], // 2
            [6], // 3
            [2], // 4
            [3], // 5
            [], // 6
        };

        int[] floorsVisited = Lift.Run(queues, DefaultCapacity);

        floorsVisited.Should().BeEquivalentTo(new[] { 0, 1, 2, 3, 4, 6, 5, 4, 3, 2, 1, 0 });
    }

    [TestMethod]
    public void Run_CollectsPeopleGoingInOppositeDirection_WhenNoOneElseToCollectFromAHigherFloor()
    {
        int[][] queues =
        {
            [], // G
            [], // 1
            [], // 2
            [1], // 3
            [], // 4
            [], // 5
            [], // 6
        };

        int[] floorsVisited = Lift.Run(queues, DefaultCapacity);

        floorsVisited.Should().BeEquivalentTo(new[] { 0, 3, 1, 0 });
    }

    [TestMethod]
    public void Run_ReturnsToCollectFromPassedFloor_WhenLiftWasOriginallyFull()
    {
        int[][] queues =
        {
            [], // G
            [], // 1
            [], // 2
            [1], // 3
            [], // 4
            [0,0], // 5
            [0,0,0], // 6
        };

        int[] floorsVisited = Lift.Run(queues, DefaultCapacity);

        floorsVisited.Should().BeEquivalentTo(new[] { 0, 6, 5, 0, 3, 1, 0 });
    }
}