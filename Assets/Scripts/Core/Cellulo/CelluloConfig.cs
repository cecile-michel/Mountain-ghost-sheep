public enum ConnectionStatus { 
        /** Idle and not connected     */ ConnectionStatusDisconnected = 0, 
        /** Actively trying to connect */ ConnectionStatusConnecting = 1,
        /** Connected                  */ ConnectionStatusConnected = 2, 
        ConnectionStatusNumElements 
}

public enum BatteryState { 
        /** No charger present, battery draining                                */ BatteryStateDischarging = 0,  
        /** No charger present, battery low, will shut down                     */ BatteryStateLow = 1, 
        /** Charger present, battery charging                                   */ BatteryStateCharging = 2, 
        /** Charger present, battery full                                       */ BatteryStateCharged = 3, 
        /** Charger charging disabled, voltage too low or battery not present   */ BatteryStateShutdown = 4, 
        /** Thermal fault or charge timeout                                     */ BatteryStateError = 5, 
        /** Number of Battery states                                            */ BatteryStateNumElements 
}

public enum LEDResponseMode {
        /** LEDs respond to touches by slightly increasing brightness   */ LEDResponseModeResponsiveIndividual = 0,
        /** LEDs don't respond to touches                               */ LEDResponseModeAbsolute = 1,
        /** LEDs respond to hold by all changing color  to green (requires Gesture Hold to be true)                */ LEDResponseModeResponsiveHold = 2,
        /** Number of LED Response Modes                                */ LEDResponseModeNumElements
}

public enum LocomotionInteractivityMode {
        /** Robot moves normally        */ LocomotionInteractivityModeNormal = 0,
        /** Robot requires Hold gesture */ LocomotionInteractivityModeRequiresHold = 1,
        /** Number of Modes             */ LocomotionInteractivityModeNumElements
}

public enum VisualEffect { 
        /** Set all LED colors (value unused)                               */ VisualEffectConstAll = 0, 
        /** Set one LED color (value is LED index)                          */ VisualEffectConstSingle =1,
        /** Alert animation for all LEDs (value unused)                     */ VisualEffectAlertAll = 2, 
        /** Alert animation for one LED (value is LED index)                */ VisualEffectAlertSingle = 3,
        /** Static progress circularly (value 0-255 maps to 0-100%)         */ VisualEffectProgress = 4, 
        /** Circular waiting/processing animation (value unused)            */ VisualEffectWaiting = 5, 
        /** Point toward one direction (value 0-255 maps to 0-360 degrees)  */ VisualEffectDirection = 6, 
        /** Alert forever (value*20 is LED on time in milliseconds)         */ VisualEffectBlink = 7, 
        /** Breathe animation (value unused)                                */ VisualEffectBreathe = 8,  
        /** Slower breathe-like animation (value unused)                    */ VisualEffectPulse = 9, 
        /** Number of Visual Effects                                        */ VisualEffectNumElements 
}

public enum Gesture {
        /** No gesture      */ GestureNone = 0,
        /** Robot is held   */ GestureHold = 1,
        GestureNumElements
}

public enum Touch{
        /** Touch Released  */ TouchReleased=0,
        /** Touch Pressed   */ TouchBegan=1,
        /** Long Touched    */ LongTouch = 2,
        TouchNumElements
}