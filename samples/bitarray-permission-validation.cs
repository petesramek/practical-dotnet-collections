using System;
using System.Collections;

// Define distinct index positions to map our boolean flags.
// This gives a readable name to the exact bit position inside the integer block.
int readIndex = 0;
int writeIndex = 1;
int deleteIndex = 2;
int adminIndex = 3;

// Initialize flag structures tracking exactly 4 independent bits.
// Note: Even though we request 4 bits, .NET always allocates at least one 
// 32-bit integer behind the scenes as its minimum memory storage size.
var userPermissionProfile = new BitArray(4);
var systemSecurityPolicy = new BitArray(4);
var evaluatedAccessResult = new BitArray(4);

// Set individual bits for the user's profile.
userPermissionProfile[readIndex] = true;
userPermissionProfile[writeIndex] = true;
userPermissionProfile[deleteIndex] = true;
userPermissionProfile[adminIndex] = false;

// Set individual bits for the system-wide security policy.
systemSecurityPolicy[readIndex] = true;
systemSecurityPolicy[writeIndex] = true;
systemSecurityPolicy[deleteIndex] = false; // This explicit false will act as a block later
systemSecurityPolicy[adminIndex] = true;

// Bulk modify evaluatedAccessResult by copying the user profile bits into it.
// This sets our starting baseline flags for evaluation.
evaluatedAccessResult.Or(userPermissionProfile);

// Intersect the user's flags with the system policy using a bulk AND operation.
// This is where BitArray shines: instead of looping through all 4 positions 
// with slow 'if' statements, the CPU evaluates the entire block of bits 
// simultaneously in a single clock cycle using hardware bitwise math.
evaluatedAccessResult.And(systemSecurityPolicy);

// Output the final results. 
// A position will only be True if both the user has it AND the system allows it.
Console.WriteLine($"Read Allowed:   {evaluatedAccessResult[readIndex]}");
Console.WriteLine($"Write Allowed:  {evaluatedAccessResult[writeIndex]}");
Console.WriteLine($"Delete Allowed: {evaluatedAccessResult[deleteIndex]}"); // Outputs False (system policy blocked it)
Console.WriteLine($"Admin Allowed:  {evaluatedAccessResult[adminIndex]}");  // Outputs False (user never had it)
