import * as http from 'https'
import * as formData from 'form-data'
import * as fs from 'fs'

const form = new FormData()
form.append('file', fs.createReadStream('./document_1.png'))

console.log(raw_data)
