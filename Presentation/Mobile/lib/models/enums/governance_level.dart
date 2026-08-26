enum GovernanceLevel{
  presidency(1),
  state(2),
  entity(3),
  canton(4),
  municipal(5);

  final int value;
  const GovernanceLevel(this.value);
}