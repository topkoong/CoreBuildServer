# Core-Build-Server
Building the core Build Server Functionality, and thoroughly testing to ensure that it functions as expected



The Executive shall construct a fixed sequence of operations of the mock Repository, mock Test Harness, and Core Project Builder, to demonstrate Builder operations.

The mock Repository shall, on command, copy a set of test source code files, test drivers, and a test request with a test for each test driver, to a path known to the Core Project Builder.

The Core Project Builder shall attempt to build each Visual Studio project delivered by the mock Repository, using the delivered code.

The Core Builder shall report, to the Console, success or failure of the build attempt, and any warnings emitted.

The Core Builder shall, on success, deliver the built library to a path known by the mock Test Harness.

The mock Test Harness shall attempt to load and execute each test library, catching any execeptions that may be emitted, and report sucess or failure and any exception messages, to the Console.
