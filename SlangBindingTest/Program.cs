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

void printIfAvailable(ISlangBlob blob)
{
    if (!blob.isNull())
    {
        Console.WriteLine(Marshal.PtrToStringAnsi(blob.getBufferPointer()));
    }
}


// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

createGlobalSession(out SlangBindings.IGlobalSession globalSession);

SessionDesc sessionDesc = new SessionDesc();
TargetDesc targetDesc = new TargetDesc();

targetDesc.format = SlangCompileTarget.SLANG_GLSL;
targetDesc.profile = globalSession.findProfile("glsl_460");
unsafe
{
    sessionDesc.targets = &targetDesc;
}
sessionDesc.targetCount = 1;
sessionDesc.defaultMatrixLayoutMode = SlangMatrixLayoutMode.SLANG_MATRIX_LAYOUT_COLUMN_MAJOR;

ISession session;
globalSession.createSession(sessionDesc, out session);

IModule testModule = session.loadModule("combined.slang", out ISlangBlob diagnosticBlob);

int entryCount = testModule.getDefinedEntryPointCount();

SlangResult pleaseWork = testModule.findEntryPointByName("fragmentMain", out IEntryPoint testEntryPoint);

SlangResult pleaseWorkAgain = testEntryPoint.link(out IComponentType finalEntryPls, out ISlangBlob diag);


finalEntryPls.getTargetCode(0, out ISlangBlob targetCode);
printIfAvailable(targetCode);
