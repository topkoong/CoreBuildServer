# Core Build Server
#### Note that in this project, I have developed core functionality of a Build Server that will grow into a Remote Build Server over the following projects. The Core Build Server source will contain packages for mock Repository and Test Harness servers that supply just enough functionality to demonstrate operations of my core Build Server. The core Build Server's Executive package serves as a local mock client, and the mock Repository and Test Harness are simply classes that provide interfaces for the executive to call, so there is no need for a communication channel. The mock servers will have just enough functionality to demonstrate Build Server operation. I, however, have not yet integrated my Build Server with a Federation's Repository and Test Harness Servers.

##### Generally, the Build Server is an automated tool that builds test libraries. Each test execution, in the Test Harness, runs a library consisting of test driver and a small set of tested packages, recording pass status, and perhaps logging execution details. Test requests and code are submitted by the Repository to the Build Server. The Build Server then builds libraries needed for each test request, and submits the request and libraries to the Test Harness, where they are executed.

- [x] Package prologues and function prologues are included.
- [x] The public interface documentation — holding a short description of the package operations, the required files to build the package, and the build process and the maintenance history — is included.
- [x] Each funtion includes some comments to describe its operation.

1. Using C#, the .Net Framework, and Visual Studio 2017.
1. Packages for an Executive, mock Repository, and mock Test Harness, as well as packages for the Core Project Builder are included.
1. The Executive constructs a fixed sequence of operations of the mock Repository, mock Test Harness, and Core Project Builder, to demonstrate Builder operations.
1. The mock Repository, on command, copies a set of test source code files, test drivers, and a test request3 with a test for each test driver, to a path known to the Core Project Builder.
1. The Core Project Builder attempts to build each Visual Studio project delivered by the mock Repository, using the delivered code.
1. The Core Builder reports to the Console, success or failure of the build attempt, and any warnings emitted.
1. The Core Builder, on success, delivers the built library to a path known by the mock Test Harness.
1. The mock Test Harness attempts to load and execute each test library, catching any execeptions that may be emitted, and report sucess or failure and any exception messages, to the Console.
1. The Core Builder, on success, delivers the built library to a path known by the mock Test Harness.

> Each package is accompanied with a test stub that serves as a construction test while building the package. 
> A Test Request is an XML file that identifies one or more tests. Each test has a test driver that implements an ITest interface and a set of packages that are associated with that test driver. So, the Mock Repository will have to know how to create Test Requests, and the Builder has to know how to parse them.

