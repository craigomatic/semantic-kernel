### Java Reference Skill (WIP)

Due to several customer requests, we want to have examples of polyglot Skills, starting with the most popular languages that customers demand. This PR implements [Java Skill Runner](https://github.com/craigomatic/semantic-kernel/blob/arbitrary-language-skills/samples/dotnet/JavaSkillRunner/Program.cs#L37), maintaining parity with [DotnetReferenceSkill](https://github.com/craigomatic/semantic-kernel/blob/arbitrary-language-skills/samples/dotnet/DotnetReferenceSkill/RandomActivitySkill.cs#L13). The Java code is invoked from C# code using `ikvm`, showcasing the interoperability between Java and C# within a single project.


### Description
Goal: [Java Skill Runner](https://github.com/craigomatic/semantic-kernel/blob/arbitrary-language-skills/samples/dotnet/JavaSkillRunner/Program.cs#L37) , running in Common Language Runtime (CLR), has to invoke a Java method (`getrandomactivity`) in `RandomActivitySkill`, running in Java Virtual Machine (JVM). `getrandomactivity` method does a simple REST call to fetch a random activity from [boredapi](https://www.boredapi.com/api/activity).

#### Architecture Decision Record (ADR)

The current implementation is not ideal and uses REST, however, I explored the following options to achieve above goal:

1. `jni4net` - Leverages Java Native Interface (JNI) methods that are exposed by the JVM to invoke Java skill. Unfortunately, JNI is [neither safe nor performant](https://developer.okta.com/blog/2022/04/08/state-of-ffi-java#java-native-interface-jni) to use. In addition, the project's recommended compatibility is [Java 5 and .NET 4](https://github.com/jni4net/jni4net#how-to-build-this-solution-on-windows), which are outdated, and the core components haven't been updated in over a decade.
   ![jni4net overview](http://jni4net.com/pics/jni4net-overview.png)
2. [ikvm](https://github.com/ikvm-revived/ikvm#run-java-applications-with-net) - This includes a JVM implemented in .NET, tool that translates JAR files to .NET IL, and cli utility to run the java application dynamically. It only support [Java SE 8 interfaces](https://github.com/ikvm-revived/ikvm#support), which is not feasible since most new Java projects are on 17, including the Java Skill in this branch.
3. [jextract from Project Panama](https://developer.okta.com/blog/2022/04/08/state-of-ffi-java#panama) - This will likely be the approach to invoke Java bytecode from C#, albeit in a year or two (w/ Java 21), since it just shipped and has to mature for aforementioned tools like ikvm to make it Foreign Function Interface (FFI) between C# and Java easy.
   ![getpid with panama](https://developer.okta.com/assets-jekyll/blog/state-of-ffi-java/panama-getpid-7b4f72ad08ccd1a024f4953dc7a08d3d983edee281611589a8ff12d23f63738c.png)
4. gRPC - This will definitely work with Protobuf's fast serialization, however, it demands a lot from the developer to generate gRPC client and server interfaces, and have both client (Java Skill) and server (Java Skill Runner) processes up, in addition to the kernel.
5. **REST (current implementation)** - Not ideal, but there's a reason why REST became the wire protocol, which certainly violates [Schillace law 7](https://learn.microsoft.com/en-us/semantic-kernel/howto/schillacelaws#consider-the-future-of-this-decidedly-semantic-ai) :) and demands more compute and open sockets.
   ```mermaid
   sequenceDiagram
    participant CLR as JavaSkillRunner (CLR)
    participant RAS as RandomActivitySkill (Java)
    participant BA as Bored API
   
    CLR->>RAS: Invoke getrandomactivity via REST
    RAS->>BA: REST GET request to Bored API
    BA->>RAS: Return random activity data
    RAS->>CLR: Return random activity data
   ```
