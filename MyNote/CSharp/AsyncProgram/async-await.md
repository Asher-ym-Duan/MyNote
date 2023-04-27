### 1. async/await的使用方法

**`异步（async）方法`**的标准写法：

```cs {.line-numbers}
方法修饰符 async 返回值 方法名(……)
{
    ……;
    Task task=…;
    //主程序遇见await时：
    //  1.跳出该异步方法，继续执行主程序后续内容
    //  2.同时异步执行任务task
    //task执行完成后，继续在该方法内往下执行
    await task;     //阻塞当前方法，不阻塞主线程
    ……;
}
```

**`async`**

一般用于修饰方法，提醒程序这是一个异步方法（可能？）

**`await`**

只能在`async`修饰的异步方法中使用，**固定用法**：`await task`

1. 主程序执行到`await task`时，主线程会暂时跳出该异步方法，执行主程序后面的代码，同时异步执行`task`
    * 该`task`到这里一般已经开始执行，如果没有开始执行，该异步方法会一直卡死
2. 等到`await task`的`task`执行完毕后，主线程才会继续进入该异步方法并且执行`await task`后面的代码

**异步方法执行完毕**：执行完异步方法内部的最后一行代码
   * 最后一行代码为`await task`时，需要等待`task`执行完毕，异步方法才算执行完毕

-----------------------------------------------------------------------


### 2. 修饰符async搭配的返回值

返回值有四种：

1. `Void`
2. `Task`
3. `Task<T>`
4. `ValueTask<T>`
   
返回值为`Void`时，仅执行异步方法，而不需要它做任何进一步的交互。
返回值为`Task`时，调用方法可以通过该返回值来检查异步方法的状态。
返回值`Task<T>`时，调用方法可以通过该返回值来检查异步方法的状态并获取一个`T`类型的值。
返回值为`ValueTask<T>`时，调用方法可以通过该返回值来获取一个`T`类型的值。


异步方法内部有无`await`的区别：
1. ==有==`await`时：（正常用法）
   * 返回值为void
   * 返回值为`Task`:
     * 方法内部不需要`return`,调用方法通过`Task task = 异步方法()`来接收异步方法，并通过`task.IsCompleted`来检查异步方法是否执行完毕
   * 返回值为`Task<T>`
     * 方法内部需要`return TResult`，调用方法通过`Task<T> task = 异步方法()`来接收异步方法，并通过`task.Result`来获取异步方法返回的T值
       * 这里注意，`task.Result`会默认等待`task`执行完毕，即等待异步方法执行完毕
   * 返回值为`ValueTask<T>`
     *  同返回值为`Task<T>` 
2. ==没有==`await`时：（编译器会警告，不影响正常执行）
   * 使用方法同有`await`  



----------------------------------------------------------------------------------------

### 3. `async`和`await`的几种情况

#### 3.1  返回值为`Void`

异步方法：

```CS {.line-numbers}
返回值
public async Void  DoAsync_Void(int x1,int x2)
{
    Task<int> task = Task.Run<int>(() => GetSum(x1,x2));
    await task;
    int sum = task.Result;
    Console.WriteLine($"End---返回值为void的异步方法--sum={sum}");
}
```

调用方法：

```CS {.line-numbers}
DoAsync_Void(1,1);
```

运行结果：

```cs {.line-numbers}
End---返回值为void的异步方法--sum=2
```

#### 3.2  返回值为`Task`

异步方法：

```CS {.line-numbers}
返回值
public async Task  DoAsync_Task(int x1,int x2)
{
    Task<int> task = Task.Run<int>(() => GetSum(x1, x2));
    await task;
    int sum = task.Result;
    Console.WriteLine($"End---返回值为Task的异步方法--sum={sum}");
}
```

调用方法：

```CS {.line-numbers}
Task task = DoAsync_Task(2,2);
Console.WriteLine($"Task.isCompleted={task.IsCompleted}----Task.Id={task.Id}");
```

运行结果：

```cs {.line-numbers}
End---返回值为Task的异步方法--sum=4
Task.isCompleted=False----Task.Id=2
```

#### 3.3  返回值为`Task<T>`

异步方法：

```CS {.line-numbers}
返回值
public async Task<int> DoAsync_TaskT(int x1, int x2)
{
    Task<int> task = Task.Run<int>(() => GetSum(x1, x2));
    await task;
    int sum = task.Result;
    Console.WriteLine($"End---返回值为Task<T>的异步方法--sum={sum}");
    return sum;
}
```

调用方法：

```CS {.line-numbers}
Task<int> task = DoAsync_TaskT(3, 3);
Console.WriteLine($"task.Result={task.Result}----task.isCompleted={task.IsCompleted}----task.Id={task.Id}");
```

运行结果：

```cs {.line-numbers}
End---返回值为Task<T>的异步方法--sum=6
task.Result=6----task.isCompleted=True----task.Id=2
```

#### 3.4  返回值为`ValueTask<T>`

异步方法：

```CS {.line-numbers}
返回值
public async ValueTask<int> DoAsync_ValueTaskT(int x1, int x2)
{
    Task<int> task = Task.Run<int>(() => GetSum(x1, x2));
    await task;
    int sum = task.Result;
    Console.WriteLine($"End---返回值为ValueTask<T>的异步方法--sum={sum}");
    return sum;
}
```

调用方法：

```CS {.line-numbers}
ValueTask<int> valueTask = DoAsync_ValueTaskT(4, 4);
Console.WriteLine($"valueTask.Result={valueTask.Result}");
```

运行结果：

```cs {.line-numbers}
End---返回值为ValueTask<T>的异步方法--sum=8
valueTask.Result=8
```

