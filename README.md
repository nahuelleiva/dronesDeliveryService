# Drones Delivery Service
WinForms application that calculates the best trip routes to deliver packages based on the drone's maximum weight capacity

# Summary
A squad of drones is tasked with delivering packages for a major online retailer in a world where time and distance do not matter.

Each drone can carry a specific weight and can make multiple deliveries before returning to home base to pick up additional packages; however, the goal is to make the fewest number of trips, as each time the drone returns to home base, it is extremely costly to refuel and reload the drone.

The software shall accept input which will include: the name of each drone, the maximum weight it can carry, along with a series of locations and the total weight needed to be delivered to that specific location. The software should highlight the most efficient deliveries for each drone to make on each trip.

Assume that time and distance to each location do not matter, and that the size of each package is irrelevant. It is also assumed that the cost to refuel and restock each drone is a constant and does not vary between drones. 
The maximum number of drones in a squad is 100, and there is no maximum number of required deliveries.

# Algorithm
For each drone, the algorithm takes its maximum weight and a list with the weights each drone can leave on each location as parameters and calculates all possible trips. 

Internally, the algorithm uses recursion and the "Gray Code" counting technique (or Reflected Binary Code -- https://en.wikipedia.org/wiki/Gray_code) looping through a binary counter that acts as a "wheel" with 1s and 0s (true or false) and recording the intermediate results everytime the bit changes. If the difference between the target sum and the intermediate result is close to zero, the algorithm saves the location weight value whose index on the wheel returns true (or 1).

# Approach
The approach is the type of solving the problem of finding a perfect sum, this is: given an array of numbers (location weights) and a target sum (drone's maximum weight), the task is to find all subsets (most efficient deliveries for each drone to make on each trip) of the given array with a sum equal to a given target sum.

Assumptions:
- Time and distance to each location don't matter.
- The size of each package is not relevant.
- Costs of refuel and restock is constant, so they can be omitted in the calculations.
- No maximum number of required delivers.
- Maximum number of drones in squad is 100.
- We assume that there are only positive values (package weight cannot be a negative number).
- Package weights are integers only.
- Each trip sum has to be equal to the drone's maximum weight capacity.
- No maximum number of required deliveries.

Technical Dependencies and Libraries:
- Visual Studio 2022
- .NET Core 6
- WinForms
