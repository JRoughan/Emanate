import uuid from 'node-uuid';
import alt from '../libs/alt';
import OutputActions from '../actions/OutputActions';

class OutputStore {
  constructor() {
    this.bindActions(OutputActions);

    this.outputs = [];

    this.exportPublicMethods({
      getOutputsByIds: this.getOutputsByIds.bind(this)
    });
  }
  create(output) {
    const outputs = this.outputs;

    output.id = uuid.v4();

    this.setState({
      outputs: outputs.concat(output)
    });

    return output;
  }
  update(updatedOutput) {
    const outputs = this.outputs.map(output => {
      if(output.id === updatedOutput.id) {
        return Object.assign({}, output, updatedOutput);
      }

      return output;
    });

    this.setState({outputs});
  }
  delete(id) {
    this.setState({
      outputs: this.outputs.filter(output => output.id !== id)
    });
  }
  getOutputsByIds(ids) {
    return (ids || []).map(
      id => this.outputs.filter(output => output.id === id)
    ).filter(a => a.length).map(a => a[0]);
  }
}

export default alt.createStore(OutputStore, 'OutputStore');
