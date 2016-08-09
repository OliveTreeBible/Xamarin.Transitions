# Xamarin Transitions
Declarative & implicit animations Library for Xamarin Forms. [![Build status](https://ci.appveyor.com/api/projects/status/7lfhk01r687406dh?svg=true)](https://ci.appveyor.com/project/adamhewitt627/xamarin-transitions)

### Nuget Packages
Project | Link
------- | ----
Common | [![NuGet Status](http://img.shields.io/nuget/v/OliveTree.Transitions.svg?style=flat)](https://www.nuget.org/packages/OliveTree.Transitions/)
Android | [![NuGet Status](http://img.shields.io/nuget/v/OliveTree.Transitions.Droid.svg?style=flat)](https://www.nuget.org/packages/OliveTree.Transitions.Droid/)
iOS | [![NuGet Status](http://img.shields.io/nuget/v/OliveTree.Transitions.iOS.svg?style=flat)](https://www.nuget.org/packages/OliveTree.Transitions.iOS/)



## Example
To automatically animate the opacity of an element, simply add the transition to its attached property. Any property changes will then be animated according to the declaration.

```XML
xmlns:trans="clr-namespace:OliveTree.Transitions;assembly=OliveTree.Transitions"

<Grid x:Name="StatusIndicator">
  <trans:Interaction.Transitions>
    <trans:TransitionCollection>
      <trans:OpacityTransition />
    </trans:TransitionCollection>
  </trans:Interaction.Transitions>

  <Grid.Triggers>
    <DataTrigger Binding="{Binding IsProcessing}" Value="False" TargetType="Grid" >
      <Setter Property="Opacity" Value="0" />
    </DataTrigger>
  </Grid.Triggers>
</Grid>
```

Or triggered in code-behind:
```C#
StatusIndicator.Opacity = 0;
```

## Setup
For the common implementation to resolve platform animations, it must be given an `ITransitionProvider`. Each platform has one built in, but you are welcome to override it should needs arise. Generally, you will initialize it with:
```C#
Forms.Init();
TransitionsLibrary.Register<Provider>();
```
