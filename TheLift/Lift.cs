namespace TheLift;

public class Lift
{
    public static int[] Run(int[][] queues, int capacity)
    {
        List<int> floorsVisited = new() { 0 };

        //HashSet<int> callingFloors = new();
        Dictionary<int, int> callingFloors = new();

        foreach (var floor in Enumerable.Range(0, queues.Length))
        {
            callingFloors.Add(floor, 0);
        }

        List<int> updatedQueue = new();
        int currentFloor = 0;

        //int currentFloor = 0;
        Direction currentDirection = Direction.Up;
        int currentOccupancy = 0;

        int remainingCallers = queues.Sum(q => q.Length);

        while (remainingCallers > 0 || currentFloor > 0)
        {
            if (currentFloor == 0)
            {
                // Embark only, lift should be empty here as it's the end of the line.
                if (queues[0].Any())
                {
                    foreach (int requestedFloor in Enumerable.Take(queues[0], capacity))
                    {
                        callingFloors[requestedFloor]++;
                        updatedQueue.Remove(requestedFloor);
                        currentOccupancy++;
                        remainingCallers--;
                    }

                    queues[0] = updatedQueue.ToArray();
                }
            }

            // Determine which floor to visit next, if any
            int? nextStop = DetermineNextStop(queues, callingFloors, currentFloor, currentDirection, currentOccupancy, capacity);

            if (nextStop is null)
            {
                // Reverse direction, as we still have remaining callers so they must be going the other way
                currentDirection = currentDirection == Direction.Up ? Direction.Down : Direction.Up;
                int floorToCheckFrom = currentDirection == Direction.Up ? 0 : queues.Length - 1;
                
                nextStop = DetermineNextStop(queues, callingFloors, floorToCheckFrom, currentDirection, currentOccupancy, capacity);

                if (nextStop is null)
                {
                    // Travel back to ground floor
                    currentFloor = 0;
                    floorsVisited.Add(0);

                    break;
                }
            }

            // Travel to the next floor and record visit
            currentFloor = nextStop!.Value;
            floorsVisited.Add(currentFloor);

            // Disembark people
            if (callingFloors.ContainsKey(currentFloor) && callingFloors[currentFloor] > 0)
            {
                // Reduce the lift occupancy by the number of people who have disembarked at this floor
                currentOccupancy -= callingFloors[currentFloor];
                callingFloors[currentFloor] = 0;
            }

            // Embark people going in the same direction as the lift
            bool peopleToEmbark = currentDirection == Direction.Up
                ? queues[currentFloor].Any(floor => floor > currentFloor)
                : queues[currentFloor].Any(floor => floor < currentFloor);
            
            if (peopleToEmbark)
            {
                updatedQueue = new(queues[currentFloor]);

                foreach (int requestedFloor in queues[currentFloor])
                {
                    if (currentDirection == Direction.Up && requestedFloor > currentFloor && currentOccupancy < capacity)
                    {
                        callingFloors[requestedFloor]++;
                        updatedQueue.Remove(requestedFloor);
                        currentOccupancy++;
                        remainingCallers--;
                    }

                    if (currentDirection == Direction.Down && requestedFloor < currentFloor && currentOccupancy < capacity)
                    {
                        callingFloors[requestedFloor]++;
                        updatedQueue.Remove(requestedFloor);
                        currentOccupancy++;
                        remainingCallers--;
                    }
                }

                queues[currentFloor] = updatedQueue.ToArray();
            }

            // Continue to next floor.

            //for (int currentFloor = 0; currentFloor < queues.Length; currentFloor++)
            //{
            //    int[] currentFloorQueue = queues[currentFloor];

            //    if (!currentFloorQueue.Any())
            //    {
            //        continue;
            //    }

            //    updatedQueue = new(currentFloorQueue);

            //    foreach (var callRequest in currentFloorQueue)
            //    {
            //        if (callRequest > currentFloor)
            //        {
            //            callingFloors.Add(callRequest);
            //            updatedQueue.Remove(callRequest);
            //            remainingCallers--;
            //        }
            //    }

            //    queues[currentFloor] = updatedQueue.ToArray();
            //}
        }

        //foreach (int[] queue in queues)
        //{
        //    if (!queue.Any())
        //    {
        //        continue;
        //    }

        //    //var validCallers = queue.Where(floorRequested => floorRequested > currentFloor)
        //    //                        .ToList();
        //}

        return floorsVisited.ToArray();
    }

    private static int? DetermineNextStop(
        int[][] queues,
        Dictionary<int, int> callingFloors,
        int currentFloor,
        Direction direction,
        int currentOccupancy,
        int capacity)
    {
        int? nextStop = null;

        if (direction == Direction.Up)
        {
            for (int potentialNextStop = currentFloor; potentialNextStop < queues.Length; potentialNextStop++)
            {
                // Stop on a floor for any existing riders who have already requested that floor
                if (callingFloors[potentialNextStop] > 0)
                {
                    return potentialNextStop;
                }

                // Stop on a floor for callers where the caller is headed in the same direction as the lift
                if (queues[potentialNextStop].Any(floor => floor > potentialNextStop) && currentOccupancy < capacity)
                {
                    return potentialNextStop;
                }
            }
        }
        else
        {
            for (int potentialNextStop = currentFloor; potentialNextStop >= 0; potentialNextStop--)
            {
                // Stop on a floor for any existing riders who have already requested that floor
                if (callingFloors[potentialNextStop] > 0)
                {
                    return potentialNextStop;
                }

                // Stop on a floor for callers where the caller is headed in the same direction as the lift
                if (queues[potentialNextStop].Any(floor => floor < potentialNextStop) && currentOccupancy < capacity)
                {
                    return potentialNextStop;
                }
            }
        }

        return nextStop;
    }
}
