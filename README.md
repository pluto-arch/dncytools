# DotnetGeek.Tools

DotnetGeek.Tools 是一个功能丰富的 .NET 工具库，提供了多种实用工具类和扩展方法，旨在提高开发效率，减少重复代码。

## 功能清单

### 1. 随机选择器（RandomSelector）
- **`WeightedItem<T>`**  
  带权重的元素封装类，用于实现加权随机选择。  
  - **属性**：
    - `Weight`：权重值。
    - `Value`：元素值。
    - `CumulativeWeight`：累计权重（内部使用）。
  - **构造函数**：
    - `WeightedItem(T value, int weight)`：初始化带权重的元素。

---

### 2. 日期工具（Date）
- **`ChineseCalendar`**  
  中国农历类，支持 1900.1.31 至 2069.12.31 的农历和节假日计算。  
  - **功能**：
    - 农历日期转换。
    - 节气、生肖、星座计算。
    - 判断是否为节假日或工作日。
  - **属性**：
    - `ChineseYear`、`ChineseMonth`、`ChineseDay`：农历年、月、日。
    - `DateHoliday`、`ChineseCalendarHoliday`：公历和农历节假日。
    - `IsHoliday`、`IsWorkDay`：是否为假期或工作日。
  - **方法**：
    - `AddDays(int days)`：增加指定天数。
    - `AddMonths(int months)`：增加指定月数。

- **`WeekHolidayStruct`**  
  表示按周计算的节假日信息。  
  - **属性**：
    - `Month`：月份。
    - `WeekAtMonth`：第几周。
    - `WeekDay`：周几。
    - `HolidayName`：节假日名称。

- **`DateInfoStruct`**  
  表示节假日的日期信息。  
  - **属性**：
    - `Month`、`Day`：月份和日期。
    - `Recess`：假期长度。
    - `HolidayName`：节假日名称。

---

### 3. 集合工具（Collection）
- **`DisposeableDictionary<TKey, TValue>`**  
  可自动释放资源的字典，继承自 `NullableDictionary<TKey, TValue>`。  
  - **功能**：
    - 自动释放 `TValue` 类型的资源（需实现 `IDisposable` 接口）。
  - **方法**：
    - `Dispose()`：释放字典中的所有资源。

- **`NullableDictionary<TKey, TValue>`**  
  支持 `null` 键的字典实现。  
  - **功能**：
    - 允许使用 `null` 作为键。
  - **构造函数**：
    - 支持多种初始化方式，包括容量和比较器。

---

### 4. 扩展方法（Extension）
- **`ObjectTypeExtensions`**  
  提供类型相关的扩展方法。  
  - **方法**：
    - `IsOneOf(Type type, params Type[] possibleTypes)`：判断类型是否属于指定类型集合。
    - `IsReferenceOrNullableType(Type type)`：判断是否为引用类型或可空类型。
    - `GetDefaultValue(Type type)`：获取类型的默认值。

- **`EnumExtension`**  
  提供枚举类型的扩展方法。  
  - **方法**：
    - `GetValueNameDictionary(Type enumType)`：获取枚举值与名称的字典。
    - `GetDescription(Enum en)`：获取枚举成员的 `Description` 属性值。

- **`RandomExtensions`**  
  提供 `Random` 类的扩展方法。  
  - **方法**：
    - `StrictNext(Random r, int maxValue)`：生成真正的随机数。
    - `NextGauss(Random rand, double mean, double stdDev)`：生成正态分布的随机数。

---

### 5. 数制转换工具（Format）
- **`NumberBaseConvertor`**  
  数制转换器，支持 2-62 进制的数值转换。  
  - **方法**：
    - `ToString(long decimalNumber)`：将十进制数转换为指定进制字符串。
    - `ToNumber(string number)`：将指定进制字符串转换为十进制数。

---

### 6. 雪花算法（SnowFlake）
- **`SnowFlake`**  
  分布式唯一 ID 生成器，基于 Twitter 的雪花算法实现。  
  - **功能**：
    - 生成全局唯一的长整型或字符串 ID。
  - **方法**：
    - `GetLongId()`：获取长整型 ID。
    - `GetUniqueId()`：获取字符串形式的 ID。

---

### 7. 对象工具（Objects）
- **`NullObject<T>`**  
  表示可空对象的结构体。  
  - **功能**：
    - 提供对 `null` 的安全封装。
  - **方法**：
    - `IsNull()`：判断对象是否为 `null`。
    - 隐式转换支持。

---


### 7. 结果模式（Result Paern）
- **`Result<T>`**  

---

## 安装方式

通过 NuGet 安装：