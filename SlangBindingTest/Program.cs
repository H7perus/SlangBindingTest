using SlangBindingTest;
using System.Runtime.InteropServices;
using System.Text;
using static SlangBindingTest.SlangBindings;

unsafe void PrintBytesAsHex(byte* address, int count)
{
    for (int i = 0; i < count; i++)
    {
        Console.Write($"{address[i]:X2} ");
        if ((i + 1) % 16 == 0) // Optional: new line every 16 bytes
            Console.WriteLine();
    }
    Console.WriteLine();
}

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

unsafe
{


    IGlobalSession globalSession;
    int hr = SlangBindings.createGlobalSession(out globalSession);

    SessionDesc sessionDesc = new SessionDesc();

    

    TargetDesc targetDesc = new TargetDesc();
    //PrintBytesAsHex((byte*)&targetDesc, 24);
    targetDesc.format = SlangCompileTarget.SLANG_GLSL;
    targetDesc.profile = globalSession.findProfile("glsl_460");

    IntPtr offset = Marshal.OffsetOf<TargetDesc>("profile");
    //Console.WriteLine($"profile offset: {offset.ToInt64()}");

    //Console.WriteLine("target profile: " + targetDesc.profile);

    //PrintBytesAsHex((byte*)&targetDesc, 24);

    sessionDesc.targetCount = 1;
    sessionDesc.targets = &targetDesc;
    sessionDesc.defaultMatrixLayoutMode = SlangMatrixLayoutMode.SLANG_MATRIX_LAYOUT_COLUMN_MAJOR;

    //Console.WriteLine(sessionDesc.structureSize);

    CompilerOptionEntry[] compilerOptions = new CompilerOptionEntry[1];

    compilerOptions[0].name = CompilerOptionName.EmitSpirvDirectly;

    compilerOptions[0].value.kind = CompilerOptionValueKind.Int;
    compilerOptions[0].value.intValue0 = 1;
    compilerOptions[0].value.intValue1 = 0;
    compilerOptions[0].value.stringValue0 = null;
    compilerOptions[0].value.stringValue1 = null;


    ISession compileSession;
    fixed (CompilerOptionEntry* ptr = &compilerOptions[0])
    {
        sessionDesc.compilerOptionEntries = null; // ptr;
        sessionDesc.compilerOptionEntryCount = 0;



        globalSession.createSession(ref sessionDesc, out compileSession);
    }

    
    

    ISlangBlob blobby;

    //IGlobalSession test = compileSession.getGlobalSession();

    IModule shaderModule = compileSession.loadModule("slangcompute.slang", out blobby);

    string slangblob = "";
    if (blobby != null)
    {
        Marshal.PtrToStringAnsi(blobby.getBufferPointer());
        Console.WriteLine(slangblob);
    }


    IEntryPoint testOut;

    SlangResult res = shaderModule.findEntryPointByName("computeMain", out testOut);

    ISlangBlob outCode;
    ISlangBlob outDiagnostics;

    shaderModule.getTargetCode(0, out outCode, out outDiagnostics);

    

    //SlangResult nonsense = shaderModule.writeToFile("yolo.txt");

    int testInt = shaderModule.getDefinedEntryPointCount();

    string targetCode = Marshal.PtrToStringAnsi(outCode.getBufferPointer());

    //string filePath = Marshal.PtrToStringUTF8(shaderModule.getFilePath());

    Console.WriteLine(targetCode);
}




//Console.WriteLine(testPtr);

//Console.WriteLine(testPtr);
//Console.WriteLine(targetDesc.profile);