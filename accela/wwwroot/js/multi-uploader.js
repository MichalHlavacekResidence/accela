const dt = new DataTransfer();
$(document).ready(function () {
    document.getElementById('pro-image').addEventListener('change', readImage, false);
    
    $(".preview-images-zone").sortable();
  
    
    $(document).on('click', '.image-cancel', function () {
        let no = $(this).data('no');
        $(".preview-image.preview-show-" + no).remove();
        //$('#pro-image').splice(no, 1);
        //console.log(no);

        console.log(dt.files.length);

        for (let i = 0; i < dt.files.length; i++) {
            var file = dt.files[i];

            if (no == file["name"]) {
                console.log(no);
                dt.items.remove(i);
            }
           
            console.log(dt);
        }

        //delete $('#pro-image').prop('files')[0];
        //$('#pro-image').prop('files').splice(no, 1)
        /*var myFile = $('#pro-image').prop('files');

        const newMyFile = [];
        for (let i = 0; i < myFile.length; i++) {
            var file = myFile[i];

            if (i == no) {
                file = null;
            }
            newMyFile.push(file);
            console.log(newMyFile);
        }
        $('#pro-image').prop('files') = newMyFile;*/


       /* myFile.toString.slice(1, 1);
        console.log(myFile);*/
      
        
       
    });
    $("#checkHaf").click(function () {
        var myFile = $('#pro-image').prop('files');
        console.log(myFile);



    });
});

var num = 0;
function readImage() {
   
    if (window.File && window.FileList && window.FileReader) {
        var files = event.target.files; //FileList object
        //console.log(files);
        var output = $(".preview-images-zone");

        for (let i = 0; i < files.length; i++) {
            var file = files[i];
            console.log(file);
            if (!file.type.match('image')) continue;
            
            var picReader = new FileReader();
            dt.items.add(file);
           // console.log(file.name);
            
            picReader.addEventListener('load', function (event) {
                var picFile = event.target;
                var html = '<div class="preview-image preview-show-' + num + '">' +
                    '<div class="image-cancel" data-no="' + num + '">x</div>' +
                    '<div class="image-zone"><img name="pro-image[]" id="pro-img-' + num + '" src="' + picFile.result + '"></div>' +
                    '<div class="tools-edit-image"><a href="javascript:void(0)" data-no="' + num + '" class="btn btn-light btn-edit-image">edit</a></div>' +
                            '</div>';

                output.append(html);
                console.log(num);
               
                num = num + 1;
            });

            picReader.readAsDataURL(file);
        }
        console.log(dt);
        //$("#pro-image").val('');
    } else {
        console.log('Browser not support');
    }
}