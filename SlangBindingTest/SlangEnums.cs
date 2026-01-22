using System;
using System.Collections.Generic;
using System.Text;

namespace SlangBindingTest
{
    public enum SlangCompileTarget : int
    {
        SLANG_TARGET_UNKNOWN,
        SLANG_TARGET_NONE,
        SLANG_GLSL,
        SLANG_GLSL_VULKAN_DEPRECATED,          //< deprecated and removed: just use `SLANG_GLSL`.
        SLANG_GLSL_VULKAN_ONE_DESC_DEPRECATED, //< deprecated and removed.
        SLANG_HLSL,
        SLANG_SPIRV,
        SLANG_SPIRV_ASM,
        SLANG_DXBC,
        SLANG_DXBC_ASM,
        SLANG_DXIL,
        SLANG_DXIL_ASM,
        SLANG_C_SOURCE,              ///< The C language
        SLANG_CPP_SOURCE,            ///< C++ code for shader kernels.
        SLANG_HOST_EXECUTABLE,       ///< Standalone binary executable (for hosting CPU/OS)
        SLANG_SHADER_SHARED_LIBRARY, ///< A shared library/Dll for shader kernels (for hosting
                                     ///< CPU/OS)
        SLANG_SHADER_HOST_CALLABLE,  ///< A CPU target that makes the compiled shader code available
                                     ///< to be run immediately
        SLANG_CUDA_SOURCE,           ///< Cuda source
        SLANG_PTX,                   ///< PTX
        SLANG_CUDA_OBJECT_CODE,      ///< Object code that contains CUDA functions.
        SLANG_OBJECT_CODE,           ///< Object code that can be used for later linking
        SLANG_HOST_CPP_SOURCE,       ///< C++ code for host library or executable.
        SLANG_HOST_HOST_CALLABLE,    ///< Host callable host code (ie non kernel/shader)
        SLANG_CPP_PYTORCH_BINDING,   ///< C++ PyTorch binding code.
        SLANG_METAL,                 ///< Metal shading language
        SLANG_METAL_LIB,             ///< Metal library
        SLANG_METAL_LIB_ASM,         ///< Metal library assembly
        SLANG_HOST_SHARED_LIBRARY,   ///< A shared library/Dll for host code (for hosting CPU/OS)
        SLANG_WGSL,                  ///< WebGPU shading language
        SLANG_WGSL_SPIRV_ASM,        ///< SPIR-V assembly via WebGPU shading language
        SLANG_WGSL_SPIRV,            ///< SPIR-V via WebGPU shading language

        SLANG_HOST_VM, ///< Bytecode that can be interpreted by the Slang VM
        SLANG_TARGET_COUNT_OF,
    };

    public enum SlangProfileID : uint
    {
        SLANG_PROFILE_UNKNOWN
    };

    public enum SlangTargetFlags : uint
    {
        /* When compiling for a D3D Shader Model 5.1 or higher target, allocate
           distinct register spaces for parameter blocks.

           @deprecated This behavior is now enabled unconditionally.
        */
        SLANG_TARGET_FLAG_PARAMETER_BLOCKS_USE_REGISTER_SPACES = 1 << 4,

        /* When set, will generate target code that contains all entrypoints defined
           in the input source or specified via the `spAddEntryPoint` function in a
           single output module (library/source file).
        */
        SLANG_TARGET_FLAG_GENERATE_WHOLE_PROGRAM = 1 << 8,

        /* When set, will dump out the IR between intermediate compilation steps.*/
        SLANG_TARGET_FLAG_DUMP_IR = 1 << 9,

        /* When set, will generate SPIRV directly rather than via glslang. */
        // This flag will be deprecated, use CompilerOption instead.
        SLANG_TARGET_FLAG_GENERATE_SPIRV_DIRECTLY = 1 << 10,
    };

    public enum SlangFloatingPointMode : uint
    {
        SLANG_FLOATING_POINT_MODE_DEFAULT = 0,
        SLANG_FLOATING_POINT_MODE_FAST,
        SLANG_FLOATING_POINT_MODE_PRECISE,
    };

    public enum SlangLineDirectiveMode : uint
    {
        SLANG_LINE_DIRECTIVE_MODE_DEFAULT =
            0,                              /**< Default behavior: pick behavior base on target. */
        SLANG_LINE_DIRECTIVE_MODE_NONE,     /**< Don't emit line directives at all. */
        SLANG_LINE_DIRECTIVE_MODE_STANDARD, /**< Emit standard C-style `#line` directives. */
        SLANG_LINE_DIRECTIVE_MODE_GLSL, /**< Emit GLSL-style directives with file *number* instead
                                           of name */
        SLANG_LINE_DIRECTIVE_MODE_SOURCE_MAP, /**< Use a source map to track line mappings (ie no
                                                 #line will appear in emitting source) */
    };

    public enum CompilerOptionName
    {
        MacroDefine, // stringValue0: macro name;  stringValue1: macro value
        DepFile,
        EntryPointName,
        Specialize,
        Help,
        HelpStyle,
        Include, // stringValue: additional include path.
        Language,
        MatrixLayoutColumn,         // bool
        MatrixLayoutRow,            // bool
        ZeroInitialize,             // bool
        IgnoreCapabilities,         // bool
        RestrictiveCapabilityCheck, // bool
        ModuleName,                 // stringValue0: module name.
        Output,
        Profile, // intValue0: profile
        Stage,   // intValue0: stage
        Target,  // intValue0: CodeGenTarget
        Version,
        WarningsAsErrors, // stringValue0: "all" or comma separated list of warning codes or names.
        DisableWarnings,  // stringValue0: comma separated list of warning codes or names.
        EnableWarning,    // stringValue0: warning code or name.
        DisableWarning,   // stringValue0: warning code or name.
        DumpWarningDiagnostics,
        InputFilesRemain,
        EmitIr,                        // bool
        ReportDownstreamTime,          // bool
        ReportPerfBenchmark,           // bool
        ReportCheckpointIntermediates, // bool
        SkipSPIRVValidation,           // bool
        SourceEmbedStyle,
        SourceEmbedName,
        SourceEmbedLanguage,
        DisableShortCircuit,            // bool
        MinimumSlangOptimization,       // bool
        DisableNonEssentialValidations, // bool
        DisableSourceMap,               // bool
        UnscopedEnum,                   // bool
        PreserveParameters, // bool: preserve all resource parameters in the output code.
        // Target

        Capability,                // intValue0: CapabilityName
        DefaultImageFormatUnknown, // bool
        DisableDynamicDispatch,    // bool
        DisableSpecialization,     // bool
        FloatingPointMode,         // intValue0: FloatingPointMode
        DebugInformation,          // intValue0: DebugInfoLevel
        LineDirectiveMode,
        Optimization, // intValue0: OptimizationLevel
        Obfuscate,    // bool

        VulkanBindShift, // intValue0 (higher 8 bits): kind; intValue0(lower bits): set; intValue1:
                         // shift
        VulkanBindGlobals,       // intValue0: index; intValue1: set
        VulkanInvertY,           // bool
        VulkanUseDxPositionW,    // bool
        VulkanUseEntryPointName, // bool
        VulkanUseGLLayout,       // bool
        VulkanEmitReflection,    // bool

        GLSLForceScalarLayout,   // bool
        EnableEffectAnnotations, // bool

        EmitSpirvViaGLSL,     // bool (will be deprecated)
        EmitSpirvDirectly,    // bool (will be deprecated)
        SPIRVCoreGrammarJSON, // stringValue0: json path
        IncompleteLibrary,    // bool, when set, will not issue an error when the linked program has
                              // unresolved extern function symbols.

        // Downstream

        CompilerPath,
        DefaultDownstreamCompiler,
        DownstreamArgs, // stringValue0: downstream compiler name. stringValue1: argument list, one
                        // per line.
        PassThrough,

        // Repro

        DumpRepro,
        DumpReproOnError,
        ExtractRepro,
        LoadRepro,
        LoadReproDirectory,
        ReproFallbackDirectory,

        // Debugging

        DumpAst,
        DumpIntermediatePrefix,
        DumpIntermediates, // bool
        DumpIr,            // bool
        DumpIrIds,
        PreprocessorOutput,
        OutputIncludes,
        ReproFileSystem,
        REMOVED_SerialIR, // deprecated and removed
        SkipCodeGen,      // bool
        ValidateIr,       // bool
        VerbosePaths,
        VerifyDebugSerialIr,
        NoCodeGen, // Not used.

        // Experimental

        FileSystem,
        Heterogeneous,
        NoMangle,
        NoHLSLBinding,
        NoHLSLPackConstantBufferElements,
        ValidateUniformity,
        AllowGLSL,
        EnableExperimentalPasses,
        BindlessSpaceIndex, // int

        // Internal

        ArchiveType,
        CompileCoreModule,
        Doc,

        IrCompression, //< deprecated

        LoadCoreModule,
        ReferenceModule,
        SaveCoreModule,
        SaveCoreModuleBinSource,
        TrackLiveness,
        LoopInversion, // bool, enable loop inversion optimization

        ParameterBlocksUseRegisterSpaces, // Deprecated
        LanguageVersion,                  // intValue0: SlangLanguageVersion
        TypeConformance, // stringValue0: additional type conformance to link, in the format of
                         // "<TypeName>:<IInterfaceName>[=<sequentialId>]", for example
                         // "Impl:IFoo=3" or "Impl:IFoo".
        EnableExperimentalDynamicDispatch, // bool, experimental
        EmitReflectionJSON,                // bool

        CountOfParsableOptions,

        // Used in parsed options only.
        DebugInformationFormat,  // intValue0: DebugInfoFormat
        VulkanBindShiftAll,      // intValue0: kind; intValue1: shift
        GenerateWholeProgram,    // bool
        UseUpToDateBinaryModule, // bool, when set, will only load
                                 // precompiled modules if it is up-to-date with its source.
        EmbedDownstreamIR,       // bool
        ForceDXLayout,           // bool

        // Add this new option to the end of the list to avoid breaking ABI as much as possible.
        // Setting of EmitSpirvDirectly or EmitSpirvViaGLSL will turn into this option internally.
        EmitSpirvMethod, // enum SlangEmitSpirvMethod

        SaveGLSLModuleBinSource,

        SkipDownstreamLinking, // bool, experimental
        DumpModule,

        GetModuleInfo,              // Print serialized module version and name
        GetSupportedModuleVersions, // Print the min and max module versions this compiler supports

        EmitSeparateDebug, // bool

        // Floating point denormal handling modes
        DenormalModeFp16,
        DenormalModeFp32,
        DenormalModeFp64,

        // Bitfield options
        UseMSVCStyleBitfieldPacking, // bool

        CountOf,
    };

    public enum CompilerOptionValueKind
    {
        Int,
        String
    };


    public struct CompilerOptionEntry
    {
        public CompilerOptionName name;
        public CompilerOptionValue value;
    };


    public enum SlangLanguageVersion : uint
    {
        SLANG_LANGUAGE_VERSION_UNKNOWN = 0,
        SLANG_LANGUAGE_VERSION_LEGACY = 2018,
        SLANG_LANGUAGE_VERSION_2025 = 2025,
        SLANG_LANGUAGE_VERSION_2026 = 2026,
        SLANG_LANGAUGE_VERSION_DEFAULT = SLANG_LANGUAGE_VERSION_LEGACY,
        SLANG_LANGUAGE_VERSION_LATEST = SLANG_LANGUAGE_VERSION_2026,
    };
    public enum SlangResult : int
    {
        SLANG_OK = 0,
        SLANG_FAIL = -1,
        // Add other specific error codes as you encounter them in slang.h
        // e.g., SLANG_E_NOT_AVAILABLE, SLANG_E_INVALID_ARG, etc.
    }
    public enum SessionFlags : uint
    {
        kSessionFlags_None = 0
    };

    public enum SlangMatrixLayoutMode : uint
    {
        SLANG_MATRIX_LAYOUT_MODE_UNKNOWN = 0,
        SLANG_MATRIX_LAYOUT_ROW_MAJOR,
        SLANG_MATRIX_LAYOUT_COLUMN_MAJOR,
    };

    public enum SlangStage : uint
    {
        SLANG_STAGE_NONE,
        SLANG_STAGE_VERTEX,
        SLANG_STAGE_HULL,
        SLANG_STAGE_DOMAIN,
        SLANG_STAGE_GEOMETRY,
        SLANG_STAGE_FRAGMENT,
        SLANG_STAGE_COMPUTE,
        SLANG_STAGE_RAY_GENERATION,
        SLANG_STAGE_INTERSECTION,
        SLANG_STAGE_ANY_HIT,
        SLANG_STAGE_CLOSEST_HIT,
        SLANG_STAGE_MISS,
        SLANG_STAGE_CALLABLE,
        SLANG_STAGE_MESH,
        SLANG_STAGE_AMPLIFICATION,
        SLANG_STAGE_DISPATCH,
        //
        SLANG_STAGE_COUNT,

        // alias:
        SLANG_STAGE_PIXEL = SLANG_STAGE_FRAGMENT,
    };

}
