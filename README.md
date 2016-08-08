# Xamarin Transitions
Declarative & implicit animations Library for Xamarin Forms.

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
    </DataTrigger
  </Grid.Triggers>
</Grid>
```

Or triggered in code-behind:
```C#
StatusIndicator.Opacity = 0;
```
