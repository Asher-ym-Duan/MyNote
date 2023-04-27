### `Lock`
只有一个线程可以访问当前方法的Lock代码块

### 线程安全对象
在对象的所属类中的属性或方法里面添加`Lock`,从而能够使得外部对象在访问该对象的属性或方法时自动上锁，但是`Lock(This)`会导致外部不能在类自身中控制这种访问，所以需要应用`SyncRoot`模式，即创建一个私有化对象`SyncRoot`，通过`Lock(SyncRoot)`来达到上锁的目的
```CS
private object SyncRoot=new object();
```

### Interlocked类