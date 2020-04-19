﻿using AssetViewer.Data;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AssetViewer {

  [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
  [TemplatePart(Name = "PART_ButtonUp", Type = typeof(ButtonBase))]
  [TemplatePart(Name = "PART_ButtonDown", Type = typeof(ButtonBase))]
  public class NumericUpDown : Control {

    #region Public Properties

    public uint MaxValue {
      get { return (uint)GetValue(MaxValueProperty); }
      set { SetValue(MaxValueProperty, value); }
    }

    public uint MinValue {
      get { return (uint)GetValue(MinValueProperty); }
      set { SetValue(MinValueProperty, value); }
    }

    public uint Increment {
      get { return (uint)GetValue(IncrementProperty); }
      set { SetValue(IncrementProperty, value); }
    }

    public uint Value {
      get { return (uint)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public ICommand Command {
      get { return (ICommand)GetValue(CommandProperty); }
      set { SetValue(CommandProperty, value); }
    }

    public object CommandParameter {
      get { return GetValue(CommandParameterProperty); }
      set { SetValue(CommandParameterProperty, value); }
    }

    #endregion Public Properties

    #region Public Fields

    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
                        "ValueChanged", RoutingStrategy.Direct,
        typeof(ValueChangedEventHandler), typeof(NumericUpDown));

    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register("MaxValue", typeof(uint), typeof(NumericUpDown), new FrameworkPropertyMetadata(uint.MaxValue, maxValueChangedCallback, coerceMaxValueCallback));

    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register("MinValue", typeof(uint), typeof(NumericUpDown), new FrameworkPropertyMetadata(uint.MinValue, minValueChangedCallback, coerceMinValueCallback));

    public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register("Increment", typeof(uint), typeof(NumericUpDown), new FrameworkPropertyMetadata(1U, null, coerceIncrementCallback));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(uint), typeof(NumericUpDown), new FrameworkPropertyMetadata(0U, valueChangedCallback, coerceValueCallback), validateValueCallback);

    public static readonly DependencyProperty CommandProperty =
      DependencyProperty.Register("Command", typeof(ICommand), typeof(NumericUpDown), new FrameworkPropertyMetadata((ICommand)null, new PropertyChangedCallback(OnCommandChanged)));

    public static readonly DependencyProperty CommandParameterProperty =
      DependencyProperty.Register("CommandParameter", typeof(object), typeof(NumericUpDown), new FrameworkPropertyMetadata((object)null));

    #endregion Public Fields

    #region Public Constructors

    static NumericUpDown() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
    }

    #endregion Public Constructors

    #region Public Events

    public event ValueChangedEventHandler ValueChanged {
      add { base.AddHandler(NumericUpDown.ValueChangedEvent, value); }
      remove { base.RemoveHandler(NumericUpDown.ValueChangedEvent, value); }
    }

    #endregion Public Events

    #region Public Methods

    public override void OnApplyTemplate() {
      base.OnApplyTemplate();

      var textBox = GetTemplateChild("PART_TextBox") as TextBox;
      if (textBox != null) {
        PART_TextBox = textBox;
        PART_TextBox.PreviewKeyDown += textBox_PreviewKeyDown;
        PART_TextBox.TextChanged += textBox_TextChanged;
        PART_TextBox.LostFocus += textBox_LostFocus;
        PART_TextBox.Text = Value.ToString();
      }
      var PART_ButtonUp = GetTemplateChild("PART_ButtonUp") as ButtonBase;
      if (PART_ButtonUp != null) {
        PART_ButtonUp.Click += buttonUp_Click;
      }
      var PART_ButtonDown = GetTemplateChild("PART_ButtonDown") as ButtonBase;
      if (PART_ButtonDown != null) {
        PART_ButtonDown.Click += buttonDown_Click;
      }
    }

    #endregion Public Methods

    #region Private Fields

    private TextBox PART_TextBox = new TextBox();

    #endregion Private Fields

    #region Private Methods

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    }

    private static object coerceMaxValueCallback(DependencyObject d, object value) {
      var minValue = ((NumericUpDown)d).MinValue;
      if ((uint)value < minValue)
        return minValue;

      return value;
    }

    private static void maxValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var numericUpDown = ((NumericUpDown)d);
      numericUpDown.CoerceValue(MinValueProperty);
      numericUpDown.CoerceValue(ValueProperty);
    }

    private static object coerceMinValueCallback(DependencyObject d, object value) {
      var maxValue = ((NumericUpDown)d).MaxValue;
      if ((uint)value > maxValue)
        return maxValue;

      return value;
    }

    private static void minValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var numericUpDown = ((NumericUpDown)d);
      numericUpDown.CoerceValue(NumericUpDown.MaxValueProperty);
      numericUpDown.CoerceValue(NumericUpDown.ValueProperty);
    }

    private static object coerceIncrementCallback(DependencyObject d, object value) {
      var numericUpDown = ((NumericUpDown)d);
      var i = numericUpDown.MaxValue - numericUpDown.MinValue;
      if ((uint)value > i)
        return i;

      return value;
    }

    private static void valueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var numericUpDown = (NumericUpDown)d;
      var control = d as NumericUpDown;

      var ea = new ValueChangedEventArgs(NumericUpDown.ValueChangedEvent, d, (uint)e.OldValue, (uint)e.NewValue);
      numericUpDown.RaiseEvent(ea);
      //if (ea.Handled) numericUpDown.Value = (uint)e.OldValue;
      //else
      numericUpDown.PART_TextBox.Text = e.NewValue.ToString();
      var command = control?.Command;
      var args = new SelectedCountChangedArgs { Count = (uint)e.NewValue, Assets = (control?.CommandParameter as IList)?.OfType<TemplateAsset>() };
      if (command?.CanExecute(args) == true)
        command.Execute(args);
    }

    private static bool validateValueCallback(object value) {
      var val = (uint)value;
      if (val >= uint.MinValue && val <= uint.MaxValue)
        return true;
      else
        return false;
    }

    private static object coerceValueCallback(DependencyObject d, object value) {
      var val = (uint)value;
      var minValue = ((NumericUpDown)d).MinValue;
      var maxValue = ((NumericUpDown)d).MaxValue;
      uint result;
      if (val < minValue)
        result = minValue;
      else if (val > maxValue)
        result = maxValue;
      else
        result = (uint)value;

      return result;
    }

    private void buttonUp_Click(object sender, RoutedEventArgs e) {
      Value += Increment;
    }

    private void buttonDown_Click(object sender, RoutedEventArgs e) {
      Value -= Increment;
    }

    private void textBox_PreviewKeyDown(object sender, KeyEventArgs e) {
      if (e.Key == Key.Space)
        e.Handled = true;
    }

    private void textBox_TextChanged(object sender, TextChangedEventArgs e) {
      var index = PART_TextBox.CaretIndex;
      uint result;
      if (!uint.TryParse(PART_TextBox.Text, out result)) {
        var changes = e.Changes.FirstOrDefault();
        PART_TextBox.Text = PART_TextBox.Text.Remove(changes.Offset, changes.AddedLength);
        PART_TextBox.CaretIndex = index > 0 ? index - changes.AddedLength : 0;
      }
      else if (result > MaxValue) {
        Value = MaxValue;
        PART_TextBox.Text = Value.ToString();
        PART_TextBox.CaretIndex = PART_TextBox.Text.Length;
      }
      else if (result < MinValue) {
        Value = MinValue;
        PART_TextBox.Text = Value.ToString();
        PART_TextBox.CaretIndex = PART_TextBox.Text.Length;
      }
      else if (result <= MaxValue && result >= MinValue)
        Value = result;
      else {
        PART_TextBox.Text = Value.ToString();
        PART_TextBox.CaretIndex = index > 0 ? index - 1 : 0;
      }
    }

    private void textBox_LostFocus(object sender, RoutedEventArgs e) {
      if (PART_TextBox.Text == string.Empty) {
        PART_TextBox.Text = "0";
      }
    }
    #endregion Private Methods
  }
}