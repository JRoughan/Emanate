import uuid from 'node-uuid';
import alt from '../libs/alt';
import OutputGroupActions from '../actions/OutputGroupActions';

class OutputGroupStore {
  constructor() {
    this.bindActions(OutputGroupActions);

    this.outputGroups = [];
  }
  create(outputGroup) {
    const outputGroups = this.outputGroups;

    outputGroup.id = uuid.v4();
    outputGroup.outputs = outputGroup.outputs || [];

    this.setState({
      outputGroups: outputGroups.concat(outputGroup)
    });
  }
  update(updatedOutputGroup) {
    const outputGroups = this.outputGroups.map(outputGroup => {
      if(outputGroup.id === updatedOutputGroup.id) {
        return Object.assign({}, outputGroup, updatedOutputGroup);
      }

      return outputGroup;
    });

    this.setState({outputGroups});
  }
  delete(id) {
    this.setState({
      outputGroups: this.outputGroups.filter(outputGroup => outputGroup.id !== id)
    });
  }
  attachToOutputGroup({outputGroupId, outputId}) {
    const outputGroups = this.outputGroups.map(outputGroup => {
      if(outputGroup.id === outputGroupId) {
        if(outputGroup.outputs.includes(outputId)) {
          console.warn('Already attached output to outputGroup', outputGroups);
        }
        else {
          outputGroup.outputs.push(outputId);
        }
      }

      return outputGroup;
    });

    this.setState({outputGroups});
  }
  detachFromOutputGroup({outputGroupId, outputId}) {
    const outputGroups = this.outputGroups.map(outputGroup => {
      if(outputGroup.id === outputGroupId) {
        outputGroup.outputs = outputGroup.outputs.filter(output => output !== outputId);
      }

      return outputGroup;
    });

    this.setState({outputGroups});
  }
}

export default alt.createStore(OutputGroupStore, 'OutputGroupStore');
