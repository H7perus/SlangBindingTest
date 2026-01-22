using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
//using static SlangBindingTest.SlangBindingsOld;

namespace SlangBindingTest
{
    internal class SlangBindings
    {
        // Ensure calling convention matches native API
        [DllImport("slang", CallingConvention = CallingConvention.Cdecl)]
        static extern SlangResult slang_createGlobalSession2(ref SlangGlobalSessionDesc desc, out IGlobalSessionPtr outGlobalSession);

        public static SlangResult createGlobalSession(out IGlobalSession outGlobalSession)
        {
            SlangGlobalSessionDesc desc = new SlangGlobalSessionDesc();
            SlangResult result = slang_createGlobalSession2(ref desc, out IGlobalSessionPtr sessionPointer);


            outGlobalSession = new IGlobalSession(sessionPointer);
            return result;
        }

        [DllImport("SlangSharpAPI", CallingConvention = CallingConvention.Cdecl)]
        static extern SlangResult IGlobalSession_createSession(ref IGlobalSessionPtr globalSession, ref SessionDesc desc, out ISessionPtr outSession);

        [DllImport("SlangSharpAPI", CallingConvention = CallingConvention.Cdecl)]
        static extern SlangProfileID IGlobalSession_findProfile(ref IGlobalSessionPtr globalSession, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("SlangSharpAPI", CallingConvention = CallingConvention.Cdecl)]
        static extern IModulePtr ISession_loadModule(ref ISessionPtr session, [MarshalAs(UnmanagedType.LPStr)] string path, out ISlangBlob diagnosticBlob);


        [DllImport("SlangSharpAPI", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ISlangBlob_getBufferPointer(ref ISlangBlobPtr blobPtr);


        [DllImport("SlangSharpAPI", CallingConvention = CallingConvention.Cdecl)]
        static extern SlangResult IModule_findEntryPointByName(ref IModulePtr module, [MarshalAs(UnmanagedType.LPStr)] string name, out IEntryPointPtr outEntryPoint);

        [DllImport("SlangSharpAPI", CallingConvention = CallingConvention.Cdecl)]
        static extern int IModule_getDefinedEntryPointCount(ref IModulePtr module);

        [DllImport("SlangSharpAPI", CallingConvention = CallingConvention.Cdecl)]
        static extern SlangResult IModule_getDefinedEntryPoint(ref IModulePtr module, int index, out IEntryPointPtr outEntryPoint);

        [DllImport("SlangSharpAPI", CallingConvention = CallingConvention.Cdecl)]
        static extern SlangResult IComponentType_getTargetCode(ref IComponentTypePtr component, long target, out ISlangBlob outBlob);

        [DllImport("SlangSharpAPI", CallingConvention = CallingConvention.Cdecl)]
        static extern SlangResult IComponentType_link(ref IComponentTypePtr component, out IComponentTypePtr outLinked, out ISlangBlob diagnostics);

        //enforce the return types for less mixup
        public struct IGlobalSessionPtr
        {
            internal IntPtr ptr = 0;

            public IGlobalSessionPtr() { }
            public IGlobalSessionPtr(IntPtr p) { ptr = p; }
        }
        public class IGlobalSession
        {
            //this is how we talk to the C++ side
            IGlobalSessionPtr Ptr;

            public IGlobalSession(IGlobalSessionPtr globalSessionPointer)
            {
                Ptr = globalSessionPointer;
            }

            public SlangResult createSession(SessionDesc desc, out ISession outSession)
            {
                // Call the native function to create a session
                SlangResult result = IGlobalSession_createSession(ref Ptr, ref desc, out ISessionPtr outSessionPtr);

                outSession = new ISession(outSessionPtr);
                return result;
            }

            public SlangProfileID findProfile(string name)
            {
                return IGlobalSession_findProfile(ref Ptr, name);
            }


        }

        public struct ISessionPtr
        {
            internal IntPtr ptr = 0;

            public ISessionPtr() { }
            public ISessionPtr(IntPtr p) { ptr = p; }
        }
        public class ISession
        {
            //this is how we talk to the C++ side
            ISessionPtr Ptr;
            public ISession(ISessionPtr sessionPointer)
            {
                Ptr = sessionPointer;
            }

            public IModule loadModule(string path, out ISlangBlob diagnosticBlob)
            {
                ISlangBlobPtr blobPtr;
                return new IModule(ISession_loadModule(ref Ptr, path, out diagnosticBlob));
            }


        }

        public struct IComponentTypePtr
        {
            internal IntPtr ptr = 0;
            public IComponentTypePtr() { }
            public IComponentTypePtr(IntPtr p) { ptr = p; }
        }

        public class IComponentType
        {
            protected IComponentTypePtr Ptr;
            public IComponentType(IComponentTypePtr componentTypePointer)
            {
                Ptr = componentTypePointer;
            }
            public SlangResult getTargetCode(long target, out ISlangBlob outBlob)
            {
                SlangResult result = IComponentType_getTargetCode(ref Ptr, target, out outBlob);
                return result;
            }

            public SlangResult link(out IComponentType returnedComponent, out ISlangBlob diagnostics)
            {
                SlangResult ret = IComponentType_link(ref Ptr, out IComponentTypePtr outComponent, out diagnostics);
                returnedComponent = new IComponentType(outComponent);
                return ret;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IModulePtr
        {
            internal IntPtr ptr = 0;
            public IModulePtr() { }
            public IModulePtr(IntPtr p) { ptr = p; }
        }

        public class IModule : IComponentType
        {
            public IModule(IModulePtr modulePointer) : base(new IComponentTypePtr(modulePointer.ptr)) { }

            public IModulePtr GetPointer()
            {
                return new IModulePtr(Ptr.ptr);
            }

            //CAREFUL: You can not get target code for the returned entry point, you must first use IComponentType.link!
            public SlangResult findEntryPointByName(string name, out IEntryPoint outEntryPoint)
            {
                IModulePtr selfPointer = GetPointer();

                SlangResult result = IModule_findEntryPointByName(ref selfPointer, name, out IEntryPointPtr entryPointPtr);
                outEntryPoint = new IEntryPoint(entryPointPtr);
                return result;
            }

            public int getDefinedEntryPointCount()
            {
                IModulePtr selfPointer = GetPointer();
                return IModule_getDefinedEntryPointCount(ref selfPointer);
            }

            //CAREFUL: You can not get target code for the returned entry point, you must first use IComponentType.link!
            public SlangResult getDefinedEntryPoint(int index, out IEntryPoint outEntryPoint)
            {
                IModulePtr selfPointer = GetPointer();
                SlangResult result = IModule_getDefinedEntryPoint(ref selfPointer, index, out IEntryPointPtr entryPointPtr);
                outEntryPoint = new IEntryPoint(entryPointPtr);
                return result;
            }
        }

        public struct IEntryPointPtr
        {
            internal IntPtr ptr = 0;
            public IEntryPointPtr() { }
            public IEntryPointPtr(IntPtr p) { ptr = p; }
        }

        public class IEntryPoint : IComponentType
        {
            public IEntryPoint(IEntryPointPtr entryPointPointer) : base(new IComponentTypePtr(entryPointPointer.ptr)) { }



        }

        public struct ISlangBlobPtr
        {
            internal IntPtr ptr = 0;
            public ISlangBlobPtr() { }
            public ISlangBlobPtr(IntPtr p) { ptr = p; }
        }

        public struct ISlangBlob
        {
            //this is how we talk to the C++ side
            ISlangBlobPtr ptr;
            public ISlangBlob(ISlangBlobPtr slangBlobPointer)
            {
                ptr = slangBlobPointer;
            }

            public bool isNull()
            {
                return ptr.ptr == IntPtr.Zero;
            }

            public IntPtr getBufferPointer()
            {
                return ISlangBlob_getBufferPointer(ref ptr);
            }
        }
    }
}
