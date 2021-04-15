using InlineIL;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading;

public class Program
{
    private class TestALC : AssemblyLoadContext
    {
        AssemblyLoadContext m_parentALC;
        public TestALC(AssemblyLoadContext parentALC) : base("test", isCollectible: true)
        {
            m_parentALC = parentALC;
        }

        protected override Assembly Load(AssemblyName name)
        {
            return m_parentALC.LoadFromAssemblyName(name);
        }
    }

    private static WeakReference<Type> s_unloadedType;
    private static WeakReference s_alc;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void LoadOnce()
    {
        if (s_alc != null && s_alc.IsAlive)
            return;

        if (s_alc != null)
            Console.WriteLine("Unloaded! {0} calls", s_numCalls);

        AssemblyLoadContext currentALC = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
        var alc = new TestALC(currentALC);
        var a = alc.LoadFromAssemblyPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Unloaded.dll"));
        Volatile.Write(ref s_unloadedType, new WeakReference<Type>(a.GetType("UnloadedType")));
        Volatile.Write(ref s_alc, new WeakReference(alc));
    }

    private static void LoaderThread()
    {
        while (true)
        {
            LoadOnce();
            Thread.Sleep(1);
        }
    }

    private static void CollectorThread()
    {
        while (true)
        {
            var wr = Volatile.Read(ref s_alc);
            while (wr != null && wr.IsAlive)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Thread.Sleep(1);
        }
    }

    public static int Main(string[] args)
    {
        new Thread(LoaderThread) { IsBackground = true }.Start();
        new Thread(CollectorThread) { IsBackground = true }.Start();

        while (true)
        {
            Stage1();
        }

        return 100;
    }

    private static int Stage1()
    {
        // Set up a tailcall dispatcher by tailcalling into Stage2.
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Emit.Tail();
        IL.Emit.Call(new MethodRef(typeof(Program), nameof(Stage2)));
        return IL.Return<int>();
    }

    private static WeakReference<IAccessor> s_accessor;
    private static int s_numCalls;
    private static int Stage2(int a0, int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14, int a15, int a16)
    {
        // Now we should have
        // Stage2
        // TailCallDispatcher
        // Stage1
        // 
        // Tailcall into Stage3, so that we have
        // Stage3
        // TailCallDispatcher
        // Stage1

        IAccessor accessor;
        WeakReference<IAccessor> accessorWr = Volatile.Read(ref s_accessor);
        if (accessorWr == null || !accessorWr.TryGetTarget(out accessor))
        {
            WeakReference<Type> unloadedTypeWr = Volatile.Read(ref s_unloadedType);
            if (unloadedTypeWr == null || !unloadedTypeWr.TryGetTarget(out Type unloadedType))
                return 0;

            accessor = (IAccessor)Activator.CreateInstance(unloadedType);
            Volatile.Write(ref s_accessor, new WeakReference<IAccessor>(accessor));
        }

        s_numCalls++;

        IL.Push(accessor);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Push(0);
        IL.Emit.Tail();
        IL.Emit.Callvirt(new MethodRef(typeof(IAccessor), "Stage3"));
        return IL.Return<int>();
    }
}
