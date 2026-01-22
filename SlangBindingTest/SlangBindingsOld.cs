using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;



public static class SlangConstants
{
    public const uint SLANG_API_VERSION = 0;
}

namespace SlangBindingTest
{



    unsafe
    internal class SlangBindingsOld
    {
        // Ensure calling convention matches native API
        [DllImport("slang", CallingConvention = CallingConvention.Cdecl)]
        static extern int slang_createGlobalSession2(ref SlangGlobalSessionDesc desc, [MarshalAs(UnmanagedType.Interface)] out IGlobalSessionOld outGlobalSession);

        // Return native HRESULT and write the IGlobalSessionOld* into the out param
        public static int createGlobalSession(out IGlobalSessionOld session)
        {
            SlangGlobalSessionDesc desc = new SlangGlobalSessionDesc();


            int hr = slang_createGlobalSession2(ref desc, out session);
            return hr;
        }




        // Base interface
        [ComImport]
        [Guid("00000000-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISlangUnknown
        {
            // PreserveSig so managed signature matches native vtable exactly (SlangResult)
            [PreserveSig]
            int QueryInterface(ref Guid riid, out IntPtr ppvObject);

            [PreserveSig]
            uint AddRef();

            [PreserveSig]
            uint Release();
        }

        [ComImport]
        [Guid("87ede0e1-4852-44b0-8bf2-cb31874de239")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISlangCastable : ISlangUnknown
        {

            /// Can be used to cast to interfaces without reference counting.
            /// Also provides access to internal implementations, when they provide a guid
            /// Can simulate a 'generated' interface as long as kept in scope by cast from.
            [PreserveSig]
            IntPtr castAs(ref Guid guid);
        };

        [ComImport]
        [Guid("8044a8a3-ddc0-4b7f-af8e-026e905d7332")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMetadata : ISlangCastable
        {
            /*
            Returns whether a resource parameter at the specified binding location is actually being used
            in the compiled shader.
            */
            [PreserveSig]
            SlangResult isParameterLocationUsed(
                /*SlangParameterCategory*/ uint category, // is this a `t` register? `s` register?
                ulong spaceIndex,            // `space` for D3D12, `set` for Vulkan
                ulong registerIndex,         // `register` for D3D12, `binding` for Vulkan
                [MarshalAs(UnmanagedType.I1)] ref bool outUsed);

            [PreserveSig]
            [return: MarshalAs(UnmanagedType.LPStr)]
            string getDebugBuildIdentifier();
        };

        [ComImport]
        [Guid("5bc42be8-5c50-4929-9e5e-d15e7c24015f")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IComponentType : ISlangUnknown
        {
            /// <summary>
            /// Get the runtime session that this component type belongs to.
            /// </summary>
            [PreserveSig]
            [return: MarshalAs(UnmanagedType.Interface)]
            ISession getSession();

            /// <summary>
            /// Get the layout for this program for the chosen targetIndex.
            /// </summary>
            [PreserveSig]
            IntPtr getLayout(
                IntPtr targetIndex,
                out ISlangBlob outDiagnostics);

            /// <summary>
            /// Get the number of (unspecialized) specialization parameters for the component type.
            /// </summary>
            [PreserveSig]
            IntPtr getSpecializationParamCount();

            /// <summary>
            /// Get the compiled code for the entry point at entryPointIndex for the chosen targetIndex.
            /// </summary>
            [PreserveSig]
            SlangResult getEntryPointCode(
                long entryPointIndex,
                long targetIndex,
                [MarshalAs(UnmanagedType.Interface)] out ISlangBlob outCode,
                [MarshalAs(UnmanagedType.Interface)] out ISlangBlob outDiagnostics);

            /// <summary>
            /// Get the compilation result as a file system.
            /// </summary>
            [PreserveSig]
            SlangResult getResultAsFileSystem(
                IntPtr entryPointIndex,
                IntPtr targetIndex,
                [MarshalAs(UnmanagedType.Interface)] out /*TODO:ISlangMutableFileSystem*/IntPtr outFileSystem);

            /// <summary>   
            /// Compute a hash for the entry point at entryPointIndex for the chosen targetIndex.
            /// </summary>
            [PreserveSig]
            void getEntryPointHash(
                IntPtr entryPointIndex,
                IntPtr targetIndex,
                out ISlangBlob outHash);

            /// <summary>
            /// Specialize the component by binding its specialization parameters to concrete arguments.
            /// </summary>
            [PreserveSig]
            SlangResult specialize(
                IntPtr specializationArgs,
                IntPtr specializationArgCount,
                [MarshalAs(UnmanagedType.Interface)] out IComponentType outSpecializedComponentType,
                out ISlangBlob outDiagnostics);

            /// <summary>
            /// Link this component type against all of its unsatisfied dependencies.
            /// </summary>
            [PreserveSig]
            SlangResult link(
                [MarshalAs(UnmanagedType.Interface)] out IComponentType outLinkedComponentType,
                out ISlangBlob outDiagnostics);

            /// <summary>
            /// Get entry point 'callable' functions accessible through the/*TODO:ISlangSharedLibrary*/IntPtr interface.
            /// </summary>
            [PreserveSig]
            SlangResult getEntryPointHostCallable(
                int entryPointIndex,
                int targetIndex,
                [MarshalAs(UnmanagedType.Interface)] out/*TODO:ISlangSharedLibrary*/IntPtr outSharedLibrary,
                out ISlangBlob outDiagnostics);

            /// <summary>
            /// Get a new ComponentType object that represents a renamed entry point.
            /// </summary>
            [PreserveSig]
            SlangResult renameEntryPoint(
                [MarshalAs(UnmanagedType.LPStr)] string newName,
                [MarshalAs(UnmanagedType.Interface)] out IComponentType outEntryPoint);

            /// <summary>
            /// Link and specify additional compiler options when generating code from the linked program.
            /// </summary>
            [PreserveSig]
            SlangResult linkWithOptions(
                [MarshalAs(UnmanagedType.Interface)] out IComponentType outLinkedComponentType,
                uint compilerOptionEntryCount,
                IntPtr compilerOptionEntries,
                out ISlangBlob outDiagnostics);

            /// <summary>
            /// Get the target code for the chosen targetIndex.
            /// </summary>
            [PreserveSig]
            SlangResult getTargetCode(
                long targetIndex,
                [MarshalAs(UnmanagedType.Interface)] out ISlangBlob outCode,
                [MarshalAs(UnmanagedType.Interface)] out ISlangBlob outDiagnostics);

            /// <summary>
            /// Get the target metadata for the chosen targetIndex.
            /// </summary>
            [PreserveSig]
            SlangResult getTargetMetadata(
                long targetIndex,
                out IntPtr outMetadata,
                [MarshalAs(UnmanagedType.Interface)] out ISlangBlob outDiagnostics);

            /// <summary>
            /// Get the entry point metadata.
            /// </summary>
            [PreserveSig]
            SlangResult getEntryPointMetadata(
                long entryPointIndex,
                long targetIndex,
                [MarshalAs(UnmanagedType.Interface)] out IMetadata outMetadata,
                [MarshalAs(UnmanagedType.Interface)] out ISlangBlob outDiagnostics);
        }

        [ComImport]
        [Guid("8F241361-F5BD-4CA0-A3AC-02F7FA2402B8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEntryPoint : IComponentType
        {
            /// <summary>
            /// Get function reflection information for this entry point.
            /// </summary>
            [PreserveSig]
            IntPtr getFunctionReflection();
        }

        [ComImport]
        [Guid("0C720E64-8722-4D31-8990-638A98B1C279")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IModule : IComponentType
        {
            [PreserveSig]
            SlangResult findEntryPointByName([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.Interface)] out IEntryPoint outEntryPoint);

            [PreserveSig]
            int getDefinedEntryPointCount();

            [PreserveSig]
            SlangResult getDefinedEntryPoint(int index, [MarshalAs(UnmanagedType.Interface)] out IEntryPoint outEntryPoint);

            [PreserveSig]
            SlangResult serialize([MarshalAs(UnmanagedType.Interface)] out ISlangBlob outSerializedBlob);

            [PreserveSig]
            SlangResult writeToFile([MarshalAs(UnmanagedType.LPStr)] string fileName);

            [PreserveSig]
            [return: MarshalAs(UnmanagedType.LPStr)]
            string getName();

            [PreserveSig]
            //[return: MarshalAs(UnmanagedType.LPStr)]
            IntPtr getFilePath();

            [PreserveSig]
            [return: MarshalAs(UnmanagedType.LPStr)]
            string getUniqueIdentity();

            [PreserveSig]
            SlangResult findAndCheckEntryPoint(
                [MarshalAs(UnmanagedType.LPStr)] string name,
                SlangStage stage,
                [MarshalAs(UnmanagedType.Interface)] out IEntryPoint outEntryPoint,
                out ISlangBlob outDiagnostics);

            [PreserveSig]
            int getDependencyFileCount();

            [PreserveSig]
            [return: MarshalAs(UnmanagedType.LPStr)]
            string getDependencyFilePath(int index);

            [PreserveSig]
            IntPtr getModuleReflection();

            [PreserveSig]
            SlangResult disassemble(out ISlangBlob outDisassembledBlob);
        }

        [ComImport]
        [Guid("8BA5FB08-5195-40e2-AC58-0D989C3A0102")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISlangBlob : ISlangUnknown
        {
            /// <summary>
            /// Get a pointer to the blob's data buffer.
            /// </summary>
            [PreserveSig]
            IntPtr getBufferPointer();

            /// <summary>
            /// Get the size of the blob's data buffer in bytes.
            /// </summary>
            [PreserveSig]
            UIntPtr getBufferSize();
        }






        // IGlobalSessionOld
        [ComImport]
        [Guid("c140b5fd-0c78-452e-ba7c-1a1e70c7f71c")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IGlobalSessionOld : ISlangUnknown
        {
            // PreserveSig to match vtable slot order & return type exactly.
            [PreserveSig]
            [return: MarshalAs(UnmanagedType.Error)]
            int createSession(
                ref SessionDesc desc,
                [MarshalAs(UnmanagedType.Interface)] out ISession outSession);

            // Native expects UTF-8 const char*; marshal as UTF-8 if runtime supports it.
            // If your runtime does not support LPUTF8Str, call FindProfileRaw instead.
            [PreserveSig]
            SlangProfileID findProfile([MarshalAs(UnmanagedType.LPStr)] string name);

            // Add more methods as needed...
        }

        // Stub interfaces for now
        [ComImport]
        [Guid("67618701-d116-468f-ab3b-474bedce0e3d")] // Placeholder GUID
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISession : ISlangUnknown
        {
            [PreserveSig]
            [return: MarshalAs(UnmanagedType.Interface)]
            IGlobalSessionOld getGlobalSession();

            /** Load a module as it would be by code using `import`.
             */
            [PreserveSig]
            [return: MarshalAs(UnmanagedType.Interface)]
            IModule loadModule([MarshalAs(UnmanagedType.LPStr)] string moduleName, [MarshalAs(UnmanagedType.Interface)] out ISlangBlob outDiagnostics);
        }


    }
}
