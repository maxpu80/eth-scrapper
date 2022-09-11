import { NextPage } from 'next';
import { ErrorMessage, Field, Form, Formik } from 'formik';
import { AddProjectData } from '../features/projects/projectsService';
import { CreateProjectError } from '../features/projects/projectModels';

export type AddResult = 'ok' | CreateProjectError;

export interface AddProjectProps {
  onAdd: (evt: AddProjectData) => Promise<AddResult>;
}

const onValidateForm = (data: AddProjectData) => {
  const errors = {} as any;

  if (!data.contractAddress) {
    errors.contractAddress = 'Required';
  } else if (false && !/^0x[a-fA-F0-9]{40}$/i.test(errors.contractAddress)) {
    errors.contractAddress = 'Not an etherium contract address';
  }

  return errors;
};

const AddProject = ({ onAdd }: AddProjectProps) => {
  return (
    <>
      <h2>Add new project</h2>
      <Formik
        initialValues={{ contractAddress: '' }}
        onSubmit={async (values, { setSubmitting, setFieldValue, setFieldError, setFieldTouched }) => {
          const result = await onAdd(values);
          setSubmitting(false);
          switch (result) {
            case 'ok':
              setFieldValue('contractAddress', '');
              setFieldTouched('contractAddress', false);
              console.log('result', result);
              break;
            case 'get-abi-error':
              setFieldError('contractAddress', 'Contract not found');
              break;
            case 'api-error':
              setFieldError('contractAddress', 'Server error try later');
              break;
          }
        }}
        validate={onValidateForm}>
        <Form>
          <Field
            type="text"
            name="contractAddress"
          />
          <ErrorMessage
            name="contractAddress"
            component="div"
          />
          <button type="submit">Add</button>
        </Form>
      </Formik>
    </>
  );
};

export default AddProject;
