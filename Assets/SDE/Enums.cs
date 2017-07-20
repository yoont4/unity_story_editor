/*
  All enums used across the Editor and it's serialized entry formats are stored here.
*/

public enum NodeType {Nothing, Dialog, Decision, SetLocalFlag, SetGlobalFlag, CheckLocalFlag, CheckGlobalFlag, Interrupt}

public enum SDEComponentType {Nothing, Node, ConnectionPoint, TextArea, Label}

public enum SDEContainerType {DialogBox, DialogInterrupt, DecisionBox}

public enum ConnectionPointType { In, Out }