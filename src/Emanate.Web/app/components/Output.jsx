import React from 'react';
import Editable from './Editable.jsx';

export default ({output, onValueClick, onEdit, onDelete}) => {
    return (
        <div>
          <Editable editing={output.editing} 
                    value={output.name} 
                    onValueClick={onValueClick.bind(null, output.id)}
                    onEdit={onEdit.bind(null, output.id)}
                    onDelete={onDelete.bind(null, output.id)} />
          <div>{output.profile.name}</div>
        </div>
  );
}



















