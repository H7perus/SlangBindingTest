using OpenGLTriangle;
using SlangBindingTest;
using System.Reflection.Metadata;
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

IModule testModule = session.loadModule("../../../combined.slang", out ISlangBlob diagnosticBlob);
printIfAvailable(diagnosticBlob);


int entryCount = testModule.getDefinedEntryPointCount();

testModule.findEntryPointByName("vertexMain", out IEntryPoint vertexEntry);
vertexEntry.link(out IComponentType vertexComponent, out ISlangBlob vertDiag);
vertexComponent.getTargetCode(0, out ISlangBlob vertexCode);


testModule.findEntryPointByName("fragmentMain", out IEntryPoint fragmentEntry);
fragmentEntry.link(out IComponentType fragmentComponent, out ISlangBlob diag);
fragmentComponent.getTargetCode(0, out ISlangBlob fragmentCode);

string vertexShader = Marshal.PtrToStringAnsi(vertexCode.getBufferPointer());

string fragmentShader = Marshal.PtrToStringAnsi(fragmentCode.getBufferPointer());



Console.WriteLine(vertexShader);
Console.WriteLine(fragmentShader);

TriangleRenderer.OpenWindow(vertexShader, fragmentShader);
