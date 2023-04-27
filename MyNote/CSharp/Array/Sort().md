`Array`类的静态方法`Sort()`主要用来排序，查看其方法列表可以发现`Sort()`有16个重载，本篇主要介绍其中三个重载的使用

---

## 1.Array.Sort(数组)
``` CS
public static void Sort<T>(T[] array); 
```
这个比较好理解，就是直接对一个数组进行元素排序（==升序==）：
1. **数值数组**：数据从小到大排序
2. **字符串数组**：字符串首字母从A到Z排序

对于元素降序，Array类并没有直接的降序方法，不过可以使用元素反转的静态方法`Reverse()`间接实现降序,具体方法为
1. 先升序 `Array.Sort(数组)`
2. 再反转 `Array.Reverse(数组)`

---
## 2.Array.Sort(数组，Comparison委托)
``` CS
public static void Sort<T>(T[] array, Comparison<T> comparison);
```

泛型委托`Comparison<T>(T x,T,y)`中定义了两个泛型参数和一个`int`类型的返回值
```CS
public delegate int Comparison<in T>(T x, T y);
```

对于返回值<0的时候，数组按照委托表达式中的关系从小到大排序（升序）
比如：

``` cs
int[] aaa = { 1, 5, 3, -1, 4, 8, -5 };
//a表示前一个元素，b表示后一个元素
Array.Sort(aaa,(a,b)=>Math.Abs(a-3)-Math.Abs(b-3));     //表示按照元素-3的绝对值从小到大排序
//Array.Sort(aaa,(a,b)=>b-a);
foreach (var item in aaa)
{
Console.Write(item+"\t");
}
```
结果所示：
``` cs
3       4       1       5       -1      8       -5
```



委托注释中对于==升序==的返回值说明为：
>一个有符号整数，表示 x 和 y 的相对值，
如下表所示。 
    //小于 0  —— x 小于 y。
    //等于0 —— x 等于 y。
    //大于 0 —— x 大于 y。

对于`int`类型数组，使元素升序时的两种定义方式如下：
1. **匿名方法**
``` CS
int[] salaryArr = { 8000, 7000, 15000, 11000, 20000 };
Array.Sort(salaryArr,delegate (int s1,int s2) {
    return s1.CompareTo(s2);
} );
```
1. **Lambda表达式**
``` CS
//Array.Sort(salaryArr, (s1, s2) => { return s2.CompareTo(s1); }); 
Array.Sort(salaryArr,(s1,s2)=> s1.CompareTo(s2));   //简便写法
```
对于比较方法`CompareTo()`，其注释中的返回值说明为：
>一个带符号的数字，指示此实例和值的相对值。
        // 返回值 – 说明
         // 小于0——这个实例小于值。
         // 0——这个实例等于值。
         // 大于0——这个实例大于值。

==降序==排序时只需要将`s1.CompareTo(s2)`替换成`s2.CompareTo(s1)`即可。

但是对于数值和字符串的升序和降序排列，使用方法`Array.Sort(数组)`和`Array.Reverse(数组)`就可以实现，所以方法`Array.Sort(数组，Comparison委托)`更多的是对自定义类的某一个属性进行排序，从而达到对类型为自定义类的数组进行排序
如：
```CS
class Student
{
    public string Name{get;set}
    public int Age{get;set;} 
    public Student(string name,int age){……}
}
//省略主方法,下同
Student[] students = {new Student("Aiden",15),new Student("Jason",16),new Student("Nick",14)};
//匿名方法
Array.Sort(students,delegate(Student s1,Student s2){return s1.Age.CompareTo(s2.Age);})
//Lambda表达式
Array.Sort(students,(s1,s2)=> s1.Age.CompareTo(s2.Age))
```


## 3.Array.Sort(数组，IComparer接口)
```CS
public static void Sort<T>(T[] array, IComparer<T>? comparer);
```