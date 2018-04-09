[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.OptionList/master/Shared/NuGet/Icon.png "Zebble.OptionList"


## Zebble.OptionList

![logo]

A Zebble plugin to allows you add an option list from a source.


[![NuGet](https://img.shields.io/nuget/v/Zebble.OptionList.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.OptionList/)

> With this plugin you can add single and multiple selection list from a data source and add some action to do when they selected or not.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.OptionList/](https://www.nuget.org/packages/Zebble.OptionList/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.
<br>


### Api Usage

To use this plugin in markup or c# code you can use below code:
```xml
<OptionsList Id="MyOptionsList" Direction="Vertical" MultiSelect="true" />
```
```csharp
await Add(new OptionsList { Id = "MyOptionsList", Direction = RepeatDirection.Vertical, MultiSelect = true });
```
<br>

##### Data source:
You can set the DataSource property (either in markup or C#) to populate the options list. The data source can be any IEnumerable object. 
```xml
<OptionsList ... DataSource="GetSource() />
```
C# code-behind:
```csharp
IEnumerable<Contacts> GetMyOptions()
{
     ....
}
```
### Properties
| Property     | Type         | Android | iOS | Windows |
| :----------- | :----------- | :------ | :-- | :------ |
| Source           | OptionsDataSource           | x       | x   | x       |
| List   | OptionsListView  | x | x | x |
| Value   | Object | x | x | x |
| DataSource   | IEnumerable<object&gt; | x | x | x |
| MultiSelect   | bool  | x | x | x |
| Direction   | RepeatDirection | x | x | x |

### Events
| Event             | Type                                          | Android | iOS | Windows |
| :-----------      | :-----------                                  | :------ | :-- | :------ |
| SelectedItemChanged              | AsyncEvent<Option&gt;    | x       | x   | x       |
