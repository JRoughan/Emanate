import assert from 'assert';
import * as types from 'app/actions/outputs';
import reducer from 'app/reducers/outputs';

describe('OutputReducer', () => {
  it('should return the initial state', () => {
    assert.equal(reducer(undefined, {}).count(), 0);
  });

  it('should create outputs', () => {
    const output = {
      id: 'foobar',
      name: 'test'
    };

    assert.deepEqual(reducer(undefined, {
      type: types.CREATE_OUTPUT,
      output: output
    }).toJS(), [output]);
  });

  it('should update outputs', () => {
    const output = {
      id: 'foobar',
      name: 'test',
      editing: true
    };
    const updatedName = 'foofoo';

    const outputs = reducer(undefined, {
      type: types.CREATE_OUTPUT,
      output: output
    });
    const state = reducer(outputs, {
      type: types.UPDATE_OUTPUT,
      id: output.id,
      name: updatedName
    });

    assert.equal(state.count(), 1);
    assert.equal(state.get(0).get('id'), output.id);
    assert.equal(state.get(0).get('name'), updatedName);
    assert.equal(state.get(0).get('editing'), output.editing);
  });

  it('should not crash while updating a non-existent output', () => {
    const output = {
      id: 'foobar',
      name: 'test'
    };

    const outputs = reducer(undefined, {
      type: types.CREATE_OUTPUT,
      output: output
    });
    const state = reducer(outputs, {
      type: types.UPDATE_OUTPUT,
      id: output.id + output.id,
      name: 'foo'
    });

    assert.equal(state.count(), 1);
    assert.equal(state.get(0).get('id'), output.id);
    assert.equal(state.get(0).get('name'), output.name);
  });

  it('should delete outputs', () => {
    const output = {
      id: 'foobar',
      name: 'test'
    };

    const outputs = reducer(undefined, {
      type: types.CREATE_OUTPUT,
      output: output
    });
    const state = reducer(outputs, {
      type: types.DELETE_OUTPUT,
      id: output.id
    });

    assert.equal(state.count(), 0);
  });

  it('should not crash while deleting a non-existent output', () => {
    const output = {
      id: 'foobar',
      name: 'test'
    };

    const outputs = reducer(undefined, {
      type: types.CREATE_OUTPUT,
      output: output
    });
    const state = reducer(outputs, {
      type: types.DELETE_OUTPUT,
      id: output.id + output.id
    });

    assert.equal(state.count(), 1);
    assert.equal(state.get(0).get('id'), output.id);
    assert.equal(state.get(0).get('name'), output.name);
  });
});
