import { ErrorMessage, Field, Form, Formik } from 'formik';
import { Result } from '../features/sharedModels';

export interface AppData {
  ethProviderUrl: string;
}

const onValidateForm = (data: AppData) => {
  const errors = {} as any;

  return errors;
};

export interface AppConfigProps {
  ethProviderUrl: string | null;
  onSetProviderUrl: (url: string) => Promise<Result<number, string>>;
}

export const AppConfig = ({ ethProviderUrl, onSetProviderUrl }: AppConfigProps) => {
  return (
    <>
      <h2>Config</h2>
      <Formik
        enableReinitialize={true}
        initialValues={{ ethProviderUrl: ethProviderUrl || '' }}
        onSubmit={async (values, { setSubmitting, setFieldValue, setFieldError, setFieldTouched }) => {
          const result = await onSetProviderUrl(values.ethProviderUrl);
          if (result.kind === 'ok') {
            setSubmitting(false);
          } else {
            setFieldError('ethProviderUrl', result.error);
          }
        }}
        validate={onValidateForm}>
        <Form>
          <Field
            type="text"
            name="ethProviderUrl"
            placeholder="to start set etherium provider url"
          />
          <ErrorMessage
            name="ethProviderUrl"
            component="div"
          />
          <button type="submit">Set</button>
        </Form>
      </Formik>
    </>
  );
};
