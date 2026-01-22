using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace SlangBindingTest
{

    [StructLayout(LayoutKind.Sequential)]
    public struct SlangGlobalSessionDesc
    {
        public uint structureSize;
        public uint apiVersion;
        public uint minLanguageVersion;

        [MarshalAs(UnmanagedType.U1)]  // C++ bool = 1 byte
        public bool enableGLSL;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public uint[] reserved;

        // Constructor to set defaults
        public SlangGlobalSessionDesc()
        {
            structureSize = (uint)Marshal.SizeOf<SlangGlobalSessionDesc>();
            apiVersion = SlangConstants.SLANG_API_VERSION;
            minLanguageVersion = (uint)SlangLanguageVersion.SLANG_LANGUAGE_VERSION_2025;
            enableGLSL = false;
            reserved = new uint[16];
        }
    }
    unsafe
    public struct CompilerOptionValue
    {
        public CompilerOptionValueKind kind = CompilerOptionValueKind.Int;
        public int intValue0 = 0;
        public int intValue1 = 0;
        public char* stringValue0 = null;
        public char* stringValue1 = null;
        public CompilerOptionValue()
        { }
    };


    public struct TargetDesc
    {
        /** The size of this structure, in bytes.
         */
        public ulong structureSize = (uint)Marshal.SizeOf<TargetDesc>();

        /** The target format to generate code for (e.g., SPIR-V, DXIL, etc.)
         */
        public SlangCompileTarget format = SlangCompileTarget.SLANG_TARGET_UNKNOWN;

        /** The compilation profile supported by the target (e.g., "Shader Model 5.1")
         */
        public SlangProfileID profile = SlangProfileID.SLANG_PROFILE_UNKNOWN;

        /** Flags for the code generation target. Currently unused. */
        public SlangTargetFlags flags = SlangTargetFlags.SLANG_TARGET_FLAG_GENERATE_SPIRV_DIRECTLY;

        /** Default mode to use for floating-point operations on the target.
         */
        public SlangFloatingPointMode floatingPointMode = SlangFloatingPointMode.SLANG_FLOATING_POINT_MODE_DEFAULT;

        /** The line directive mode for output source code.
         */
        public SlangLineDirectiveMode lineDirectiveMode = SlangLineDirectiveMode.SLANG_LINE_DIRECTIVE_MODE_DEFAULT;

        /** Whether to force `scalar` layout for glsl shader storage buffers.
         */
        public bool forceGLSLScalarBufferLayout = false;

        /** Pointer to an array of compiler option entries, whose size is compilerOptionEntryCount.
         */
        unsafe
        public CompilerOptionEntry* compilerOptionEntries = null;

        /** Number of additional compiler option entries.
         */
        public uint compilerOptionEntryCount = 0;

        public TargetDesc()
        {
        }

    };


    public struct PreprocessorMacroDesc
    {
        IntPtr name;
        IntPtr value;
    };
    public struct SessionDesc
    {
        /** The size of this structure, in bytes.
         */
        public uint structureSize = (uint)Marshal.SizeOf<SessionDesc>();

        /** Code generation targets to include in the session.
         */
        unsafe
        public TargetDesc* targets = null;

        public long targetCount = 0;

        /** Flags to configure the session.
         */
        public SessionFlags flags = SessionFlags.kSessionFlags_None;

        /** Default layout to assume for variables with matrix types.
         */
        public SlangMatrixLayoutMode defaultMatrixLayoutMode = SlangMatrixLayoutMode.SLANG_MATRIX_LAYOUT_ROW_MAJOR;

        /** Paths to use when searching for `#include`d or `import`ed files.
         */
        public IntPtr searchPaths = 0;


        public long searchPathCount = 0;

        unsafe
        public PreprocessorMacroDesc* preprocessorMacros = null;

        public long preprocessorMacroCount = 0;
        
        //TODO: There is a file system struct in slang c++
        public IntPtr fileSystem = 0;

        public bool enableEffectAnnotations = false;
        public bool allowGLSLSyntax = false;

        /** Pointer to an array of compiler option entries, whose size is compilerOptionEntryCount.
         */
        unsafe
        public CompilerOptionEntry* compilerOptionEntries = null;

        /** Number of additional compiler option entries.
         */
        public uint compilerOptionEntryCount = 0;

        /** Whether to skip SPIRV validation.
         */
        public bool skipSPIRVValidation = false;

        public SessionDesc() { }
    };

}
