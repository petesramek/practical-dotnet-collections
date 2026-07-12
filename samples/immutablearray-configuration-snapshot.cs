using System;
using System.Collections.Immutable;

// Setup a baseline block of critical system monitoring thresholds.
int[] criticalHardwareThresholds = new int[] { 75, 80, 85, 90 };

// Initialize an absolute read-only memory segment snapshot.
// ImmutableArray<T> is a lightweight value-type struct wrapper around a raw array block.
var baselineSnapshot = criticalHardwareThresholds.ToImmutableArray();

// USE CASE: Safe structural expansion using an allocation-optimized Builder path.
// Instead of modifying an immutable array entry-by-entry inside a loop (which copies the 
// entire underlying array memory structure on every single loop iteration), the companion 
// Builder is used to perform high-speed linear mutations completely allocation-free.
var structuralSnapshotBuilder = baselineSnapshot.ToBuilder();

int[] dynamicEmergencyFailsafes = new int[] { 95, 98, 100 };
foreach (int boundaryValue in dynamicEmergencyFailsafes) {
    // Mutates the internal intermediate array state cleanly without generating memory trash
    structuralSnapshotBuilder.Add(boundaryValue);
}

// Emits a frozen, thread-safe, and lock-free immutable memory segment block
var finalHardenedSnapshot = structuralSnapshotBuilder.ToImmutable();

Console.WriteLine($"Baseline Configuration Rules Tracked: {baselineSnapshot.Length}");
Console.WriteLine($"Hardened Target Emergency Triggers:   {finalHardenedSnapshot.Length}");

// Any thread can read these segments simultaneously with direct, raw array performance
Console.WriteLine($"Direct Memory Index [4] Active Threshold: {finalHardenedSnapshot[4]}");
