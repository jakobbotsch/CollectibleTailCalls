using InlineIL;
using System;
using System.Runtime.CompilerServices;

public class UnloadedType : IAccessor
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int Stage3(int a0, int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14, int a15, int a16, int a17)
    {
        // Tailcall into Stage4 that has no 'this' alive.
        // This will set up a CallTailCallTarget associated with this loader and return to the original assembly.
        // At that point we might risk having only a function pointer into this assembly, and thus we require something
        // else to keep it alive.
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
        IL.Push(0);
        IL.Push(0);
        IL.Emit.Tail();
        IL.Emit.Call(new MethodRef(typeof(UnloadedType), nameof(Stage4)));
        return IL.Return<int>();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Stage4(int a0, int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14, int a15, int a16, int a17, int a18, int a19)
    {
        return 0;
    }
}
